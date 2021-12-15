Execute these commands in order to run a MongoDB cluster with replica set in containers:

- docker-compose up -d
- docker ps -a
- docker exec -it mongo1 mongo
- config={_id:"mongo-set",members:[{_id:0,host:"mongo1:27017"},{_id:1,host:"mongo2:27017"},{_id:2,host:"mongo3:27017"}]};
- rs.initiate(config);
- exit