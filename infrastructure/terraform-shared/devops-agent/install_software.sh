#!/bin/sh

#strict mode, fail on error
set -euo pipefail

# Install Python 3 packages for installing Taurus
apt-get install python3-pip python3-dev

# Install JDK for running JMeter
apt-get install openjdk-8-jdk

# Install the .NET Core SDK, for test coverage report generation
wget -q https://packages.microsoft.com/config/ubuntu/18.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
sudo add-apt-repository universe
sudo apt-get update
sudo apt-get install apt-transport-https
sudo apt-get update
sudo apt-get install dotnet-sdk-3.1
