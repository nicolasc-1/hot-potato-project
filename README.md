# Welcome to the Hot Potato Project!
## Purpose
blabla

## Parameters
- NB_INSTANCES: number of instances to create for the test
- MODE: n/a
- ENVOY_PORT: port on which envoy will listen (for direct communication mode)

## List of commands

#### Generate docker compose stack file
```docker compose --file docker-compose.cli.yml up```

Note: to change the number of instances created, change the value of NB_INSTANCES in docker-compose.cli.yml.

#### Start stack
```docker compose --file docker-compose.stack.yml up -d```

#### Stop stack
```docker compose --file docker-compose.stack.yml stop```