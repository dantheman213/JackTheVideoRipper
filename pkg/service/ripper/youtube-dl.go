package ripper

import (
    "encoding/json"
    "fmt"
    "github.com/dantheman213/go-cli"
    "io/ioutil"
)

var MediaDir string = "/collection"

type YouTubeDlMetaModel struct {
    Extractor string  `json:"extractor"`
    Format string `json:"format"`
    Description string `json:"description"`
    Thumbnail string `json:"thumbnail"`
    Filename string `json:"_filename"`
    Title string `json:"title"`
    Ext string `json:"ext"`
}

func DownloadMeta(url string) (*YouTubeDlMetaModel, error) {
    command := fmt.Sprintf("youtube-dl -s --no-warnings --no-cache-dir --print-json %s", url)
    _, buf, err := cli.MakeAndRunCommandWithCombinedOutputThenWait(command)
    if err != nil {
        return nil, err
    }

    bytes, err := ioutil.ReadAll(buf)
    if err != nil {
        return nil, err
    }

    var model YouTubeDlMetaModel
    if err = json.Unmarshal(bytes, &model); err != nil {
        return nil, err
    }

    return &model, nil
}

func DownloadAudio() {

}

func DownloadVideo(url, localFileName string) error {
    command := fmt.Sprintf("cd %s && youtube-dl --no-check-certificate --prefer-ffmpeg --no-warnings --restrict-filenames --embed-thumbnail --add-metadata -o %s %s", MediaDir, localFileName, url)
    _, _, err := cli.MakeAndRunCommandWithCombinedOutput(command)
    if err != nil {
        return err
    }

    return nil
}
