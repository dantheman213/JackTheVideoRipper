# JackTheVideoRipper

TODO

## Getting Started

### Build Docker Image
```
docker build -t jtvr:latest .
```

### Run Application
```
docker run --rm -d --name jtvp \
-e JTVR_PORT=8080 \
-e JTVR_DATA_RETENTION_PERIOD=0 \
-v /host/path/to/collection:/collection \
-p 8080:8080 \
jtvr:latest
```
