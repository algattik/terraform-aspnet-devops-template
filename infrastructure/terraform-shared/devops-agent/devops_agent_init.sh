#!/bin/sh


test -n "$1" || { echo "The argument az_devops_url must be provided"; exit 1; }
az_devops_url="$1"
[[ "$az_devops_url" == */ ]] || { echo "The argument az_devops_url must end with /"; exit 1; }
test -n "$2" || { echo "The argument az_devops_pat must be provided"; exit 1; }
az_devops_pat="$2"
test -n "$3" || { echo "The argument az_devops_agent_pool must be provided"; exit 1; }
az_devops_agent_pool="$3"
test -n "$4" || { echo "The argument az_devops_agents_per_vm must be provided"; exit 1; }
az_devops_agents_per_vm="$4"


#strict mode, fail on error
set -euo pipefail


echo "start"

if test -e /dev/sdc && ! test -f /var/lib/docker-mounted-data-disk; then

  echo "Configure data disk /dev/sdc as Docker data directory"

  if test ! -e /dev/sdc1; then

    echo "Formatting data disk"

    (
    echo n # Add a new partition
    echo p # Primary partition
    echo 1 # Partition number
    echo   # First sector (Accept default)
    echo   # Last sector (Accept default)
    echo p # Print partition table
    echo w # Write changes
    ) | fdisk /dev/sdc

    partprobe
  fi

  mkfs -t xfs /dev/sdc1

  mkdir /var/lib/docker
  mount /dev/sdc1 /var/lib/docker

  var=$(blkid /dev/sdc1 -s UUID | awk -F'UUID="|"' '{print $2}')
  echo >> /etc/fstab "UUID=$var /var/lib/docker xfs defaults,nofail 1 2"
  touch /var/lib/docker-mounted-data-disk

fi

echo "install Ubuntu packages"

# To make it easier for build and release pipelines to run apt-get,
# configure apt to not require confirmation (assume the -y argument by default)
export DEBIAN_FRONTEND=noninteractive
echo 'APT::Get::Assume-Yes "true";' > /etc/apt/apt.conf.d/90assumeyes
echo 'Dpkg::Use-Pty "0";' > /etc/apt/apt.conf.d/00usepty


apt-get update
apt-get install -y --no-install-recommends \
        ca-certificates \
        jq \
        apt-transport-https \
        docker.io \
        curl \
        unzip

echo "Configuring regular cleanup of cached Docker images and data"

cat > /etc/cron.d/delete_old_docker_images << EOF
01 * * * * root docker system prune --all --force --filter "until=24h"
EOF

echo "Allowing agent to run docker"

usermod -aG docker azuredevopsuser

echo "Installing Azure CLI"

curl -sL https://aka.ms/InstallAzureCLIDeb | bash

echo "install VSTS Agent"

cd /home/azuredevopsuser
mkdir -p agent
cd agent

AGENTRELEASE="$(curl -s https://api.github.com/repos/Microsoft/azure-pipelines-agent/releases/latest | grep -oP '"tag_name": "v\K(.*)(?=")')"
AGENTURL="https://vstsagentpackage.azureedge.net/agent/${AGENTRELEASE}/vsts-agent-linux-x64-${AGENTRELEASE}.tar.gz"
echo "Release "${AGENTRELEASE}" appears to be latest"
echo "Downloading..."
wget -q -O agent_package.tar.gz ${AGENTURL}

for agent_num in $(seq 1 $az_devops_agents_per_vm); do
  agent_dir="agent-$agent_num"
  mkdir -p "$agent_dir"
  pushd "$agent_dir"
    agent_id="${HOSTNAME}_${agent_num}"
    echo "installing agent $agent_id"
    tar zxf ../agent_package.tar.gz
    chmod -R 777 .
    echo "extracted"
    ./bin/installdependencies.sh
    echo "dependencies installed"

    if test -e .agent; then
      echo "attempting to uninstall agent"
      ./svc.sh stop || true
      ./svc.sh uninstall || true
      sudo -u azuredevopsuser ./config.sh remove --unattended --auth pat --token "$az_devops_pat" || true
    fi

    echo "running installation"
    sudo -u azuredevopsuser ./config.sh --unattended --url "$az_devops_url" --auth pat --token "$az_devops_pat" --pool "$az_devops_agent_pool" --agent "$agent_id" --acceptTeeEula --work ./_work --runAsService
    echo "configuration done"
    ./svc.sh install
    echo "service installed"
    ./svc.sh start
    echo "service started"
    echo "config done"
  popd
done
