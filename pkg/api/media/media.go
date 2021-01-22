package media

import (
	"github.com/dantheman213/JackTheVideoRipper/pkg/models"
	"github.com/dantheman213/JackTheVideoRipper/pkg/service/ripper"
	"github.com/labstack/echo/v4"
)

func SetupRoutes(e *echo.Echo) {
	e.POST("/v1/media", func(c echo.Context) error {
		model := new(models.MediaPostRequestSchema)
		if err := c.Bind(&model); err != nil {
			return c.NoContent(400)
		}

		// TODO
		ripper.DownloadVideo(model.MediaUrl)

		return c.NoContent(201)
	})
}
