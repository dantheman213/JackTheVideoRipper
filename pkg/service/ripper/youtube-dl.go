package ripper

import (
    "fmt"
    "github.com/dantheman213/go-cli"
    log "github.com/sirupsen/logrus"
)

var MediaDir string = "/collection"

func DownloadVideo(url string) {
    command := fmt.Sprintf("cd %s && youtube-dl --no-check-certificate --prefer-ffmpeg --no-warnings --restrict-filenames --embed-thumbnail --add-metadata %s", MediaDir, url)
    _, _, err := cli.MakeAndRunCommandWithCombinedOutput(command)
    if err != nil {
        log.Error(err)
    }
}

func DownloadAudio() {

}