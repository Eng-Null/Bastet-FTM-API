namespace BastetAPI.Settings
{
    //create docker network
    //docker network create RedWolf

    //Build Project in Docker
    //docker build -t bastetftm:v4 .

    //Run mongodb in docker
    //docker run -d --rm --name mongo -p 27017:27017 -v mongodbdata:/data/db -e MONGO_INITDB_ROOT_USERNAME=redwolf -e MONGO_INITDB_ROOT_PASSWORD=Red^1992580 --network=RedWolf mongo

    //run api in docker
    //docker run -it --rm -p 8080:80 -e MongoDbSetting:Host=mongo -e MongoDbSetting:Password=Red^1992580 --network=RedWolf bastetftm:v1

    //List docker containers
    //docker ps

    //apply the yaml files to docker
    //cd .\Kubernetes\
    //kubectl apply -f .\BastetFTM.yaml
    //to Delete the StatefulSet
    //kubectl delete -f .\BastetFTM.yaml

    //kubectl create secret generic bastet-secrets --from-literal=mongodb-password='Red^1992580'
    //dotnet user-secrets set MongoDbSettings:Password Red^1992580
    public class MongoDbSetting
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string ConnectionString => $"mongodb://{User}:{Password}@{Host}:{Port}";
    }
}
