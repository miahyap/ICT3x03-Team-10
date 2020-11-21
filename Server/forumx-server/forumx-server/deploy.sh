sudo docker build -t forumx-server .
sudo docker stop forumxapp
sudo docker rm  forumxapp
sudo docker run -d --name forumxapp forumx-server