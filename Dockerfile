# prepare the base image that will be used in final product
FROM debian:buster-slim as base

WORKDIR /tmp
RUN apt-get update

# common dependencies
RUN apt-get install -y curl ffmpeg

# install youtube-dl and dependencies
RUN apt-get install -y python3 && \
    ln -s /usr/bin/python3 /usr/bin/python
RUN curl -L https://yt-dl.org/downloads/latest/youtube-dl -o /usr/local/bin/youtube-dl && \
    chmod a+rx /usr/local/bin/youtube-dl
RUN youtube-dl -U

# compile application in a workspace
FROM golang:1.15 as workspace

WORKDIR /go/src/app
COPY . .

RUN make deps
RUN make

# prepare finished image by combining base image and assets from workspace
FROM base as release
COPY --from=workspace /go/src/app/bin/JackTheVideoRipper /usr/bin/JackTheVideoRipper
RUN chmod +x /usr/bin/JackTheVideoRipper

ENTRYPOINT ["/usr/bin/JackTheVideoRipper"]
