package media

import (
	"github.com/dantheman213/JackTheVideoRipper/pkg/models"
	"github.com/dantheman213/JackTheVideoRipper/pkg/service/ripper"
	"github.com/labstack/echo/v4"
	log "github.com/sirupsen/logrus"
)

func SetupRoutes(e *echo.Echo) {
	e.POST("/v1/media", func(c echo.Context) error {
		model := new(models.MediaPostRequestSchema)
		if err := c.Bind(&model); err != nil {
			return c.NoContent(400)
		}

		// TODO
		if err := ripper.DownloadVideo(model.MediaUrl, "todo.mp4"); err != nil {
			log.Error(err)
			return c.NoContent(500)
		}

		return c.NoContent(201)
	})
}
