docker build -t tamagotchi-image .

docker tag tamagotchi-image registry.heroku.com/tamagotchi-api/web

docker push registry.heroku.com/tamagotchi-api/web

heroku container:release web -a tamagotchi-api
