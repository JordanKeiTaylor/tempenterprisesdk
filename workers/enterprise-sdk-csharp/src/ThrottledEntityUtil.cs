using System;
using System.Linq;
using System.Threading.Tasks;
using Improbable.Worker;

namespace Improbable.Enterprise
{
    public class ThrottledEntityUtil : EntityUtil {
        internal class TaskQueue : IDisposable {
            internal class TaskAndState {
                public Task Task;
                public bool Running;
            }

            private System.Collections.Generic.List<TaskAndState> _queue;
            private readonly uint _concurrentTasks;
            private bool stoppingQueue = false;

            public TaskQueue(uint concurrentTasks) {
                _queue = new System.Collections.Generic.List<TaskAndState>();
                _concurrentTasks = concurrentTasks;
            }

            public void Dispose() {
                System.Collections.Generic.List<TaskAndState> runningTasks;
                lock (_queue) {
                    runningTasks = _queue.Where(task => task.Running).ToList();
                    stoppingQueue = true;
                }
                Task.WaitAll(runningTasks.Select(task => task.Task).ToArray());
            }

            public Task<T> Enqueue<T>(Task<T> task) {
                lock (_queue) {
                    if (stoppingQueue) {
                        return task;
                    }
                    _queue.Add(new TaskAndState() {
                            Task = new Task(() => {
                                task.Start();
                                task.Wait();
                                DoneTask(Task.CurrentId ?? -1);
                                StartTask();
                            }),
                            Running = false
                        }
                    );
                }
                StartTask();
                return task;
            }

            private void DoneTask(int doneTaskId) {
                lock (_queue) {
                    _queue = _queue.Where(task => task.Task.Id != doneTaskId).ToList();
                }
            }

            public void StartTask() {
                lock (_queue) {
                    var numRunningTasks = 0;
                    foreach (var task in _queue) {
                        if (task.Running == false
                            && numRunningTasks < _concurrentTasks
                            && !stoppingQueue) {
                            numRunningTasks++;
                            task.Task.Start();
                            task.Running = true;
                        } else if (task.Running) {
                            numRunningTasks++;
                        }
                    }
                }
            }
        }

        private TaskQueue _taskQueue;
        public ThrottledEntityUtil(uint concurrentQueries,
            RequestDispatcher requestDispatcher, View view)
            : base(requestDispatcher, view) {
            _taskQueue = new TaskQueue(concurrentQueries);
        }

        public override void Dispose() {
            _taskQueue.Dispose();
            base.Dispose();
        }

        protected override Task<T> SendEntityQuery<T>(Task<T> queryTask) {
            return _taskQueue.Enqueue(queryTask);
        }
    }
}