#!/usr/bin/env bash

cd worker
gradle upload -PnexusUser=$1 -PnexusPassword=$2
