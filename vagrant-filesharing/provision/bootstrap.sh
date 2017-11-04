#!/usr/bin/env bash

# nginx
sudo apt-get -y install nginx
sudo service nginx start

# set up nginx server
sudo cp /vagrant/provision/nginx/default /etc/nginx/sites-enabled/default
sudo chmod 644 /etc/nginx/sites-enabled/default
sudo service nginx restart

# clean /var/www
sudo rm -Rf /var/www

# symlink /var/www => /vagrant
ln -s /vagrant /var/www

#install dotnet
sudo apt-get update
sudo apt-get -y install curl libunwind8 gettext apt-transport-https --force-yes
curl https://packages.microsoft.com/keys/microsoft.asc | gpg --dearmor > microsoft.gpg
sudo mv microsoft.gpg /etc/apt/trusted.gpg.d/microsoft.gpg
sudo sh -c 'echo "deb [arch=amd64] https://packages.microsoft.com/repos/microsoft-debian-stretch-prod stretch main" > /etc/apt/sources.list.d/dotnetdev.list'
sudo apt-get update
sudo apt-get -y install dotnet-sdk-2.0.2

#deploy FileSharingWeb
sudo mkdir -p /var/filesharing/{app,logs,folders}
sudo cp -r /vagrant/provision/FileSharing/* /var/filesharing/app/
sudo cp /vagrant/provision/FileSharingConfig/* /var/filesharing/app/
sudo chown -R www-data:www-data /var/filesharing
sudo cp /vagrant/provision/systemd/filesharing.service /etc/systemd/system/filesharing.service
sudo systemctl enable filesharing.service
sudo systemctl start filesharing.service