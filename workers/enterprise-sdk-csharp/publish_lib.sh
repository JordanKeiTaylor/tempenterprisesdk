#!/bin/sh

# run this script with the first arg as the nuget api key from nexus

nuget pack EnterpriseSDKCSharp.nuspec
nuget push EnterpriseSDK.10.0.0.nupkg $1 -Source http://nexus.enterprise.improbable.io:8080/repository/nuget-hosted/