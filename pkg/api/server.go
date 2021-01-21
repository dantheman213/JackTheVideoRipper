package api

import (
	"fmt"
	"github.com/dantheman213/JackTheVideoRipper/pkg/media"
	"github.com/labstack/echo/v4"
	"github.com/labstack/gommon/log"
)

var port int = 8080

func Start() *echo.Echo {
	log.Info("initializing web server")
	e := echo.New()
	e.HideBanner = true

	media.SetupRoutes(e)

	log.Infof("starting on port %d", port)
	if err := e.Start(fmt.Sprintf(":%d", port)); err != nil {
		log.Error(err)
	}

	return e
}
