#EnterpriseSDK

##EnterpriseSDK for Java

This is a library that wraps the SpatialOS Java worker sdk in order kickstart development.

###Source

workers/enterprise-sdk-java

###Making changes

When making changes please
- update the version tag in workers/enterprise-sdk-java/worker/build.gradle
	- use Semantic Versioning (http://semver.org/).

###Publishing to Nexus
- Run workers/enterprise-sdk-java/publish_lib.sh with arg1 being nexus username and arg2 being nexus password

##EnterpriseSDK for C#

This is a library that wraps the SpatialOS C# worker sdk in order kickstart development.

###Source

workers/enterprise-sdk-csharp

###Making changes

When making changes please
- update the version tag in workers/enterprise-sdk-csharp/EnterpriseSDKCSharp.nuspec
	- use Semantic Versioning (http://semver.org/).
- update the releaseNotes in workers/enterprise-sdk-csharp/EnterpriseSDKCSharp.nuspec
	- Make sure to include the SpatialOS worker sdk version that this lib is built against.

###Publishing to Nexus
- Run workers/enterprise-sdk-csharp/publish_lib.sh with appropriate api key for nexus user as arg1