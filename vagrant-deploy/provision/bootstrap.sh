#!/usr/bin/env bash

# nginx
sudo apt-get -y install nginx
sudo service nginx start

# set up nginx server
sudo cp /vagrant/provision/nginx/default /etc/nginx/sites-available/default
sudo chmod 644 /etc/nginx/sites-available/default
sudo ln -s /etc/nginx/sites-available/default /etc/nginx/sites-enabled/default
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
sudo sh -c 'echo "deb [arch=amd64] https://packages.microsoft.com/repos/microsoft-debian-jessie-prod jessie main" > /etc/apt/sources.list.d/dotnetdev.list'
sudo apt-get update
sudo apt-get -y install dotnet-sdk-2.0.2

#configure systemd
sudo mkdir -p /var/aspnetcore/
sudo cp -R /vagrant/provision/FileSharing /var/aspnetcore/
sudo cp /vagrant/provision/systemd/filesharing.service /etc/systemd/system/filesharing.service
sudo systemctl enable filesharing.service
sudo systemctl start filesharing.service