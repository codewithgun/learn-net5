## Generate Dockerfile

1. Install docker extension for visual studio code
2. Press `Ctrl + Shift + P`
3. Search for `Add Docker Files to workspace`
4. Select ASP.NET Core
5. Select Linux as Operating System
6. Enter `80` port
7. Select `No` for generating docker-compose file

## Building docker image

1. docker build -t {image_name}:{tag}. Eg: docker build -t catalog:v0.0.1

## Push docker image to docker-hub

1. docker login
2. docker tag {built_image_name}:{built_image_tag} {docker_authenticated_name}/{image_name_on_hub}:{image_tag_on_hub}
3. docker push {docker_authenticated_name}/{image_name_on_hub}:{image_tag_on_hub}

## Docker housekeeping

1. docker image prune
2. docker network prune
3. docker container prune
4. docker volume prune
5. All-in-one: docker system prune

## Docker compose

- The label under services, was the container name, but docker-compose will automatically add prefix and suffix to it.
- The version in docker compose file, declare the docker-compose file format version.
- We can have multiple docker compose file, and running them by using `docker-compose -f {file_name.yaml} -d`

## Docker network

If two images was listed in the same docker yaml file, docker engine will automatically place them in the same docker network.

## Docker volume

### Host volume

When mounting container virtual file to host system, we need to explicitly specify the path and the name. Eg:
`-v ${path_in_host_system}:{path_in_container}`

### Anonymous volume

When mounting, the docker automatically generate random hash for it. The file will be located at `/var/lib/docker/volumes/${random_hash}/_data` Eg: `-v ${path_in_container}`

### Named volume (Recommended in production)

Similar to anonymous, except we need to specify the name of the folder.
Eg: `-v ${name}:${path_in_container}`
