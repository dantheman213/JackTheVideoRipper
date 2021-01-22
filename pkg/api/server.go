package api

import (
	"fmt"
	"github.com/dantheman213/JackTheVideoRipper/pkg/api/media"
	"github.com/go-playground/validator/v10"
	"github.com/labstack/echo/v4"
	log "github.com/sirupsen/logrus"
)

var port int = 8080

type CustomValidator struct {
	validator *validator.Validate
}

func (cv *CustomValidator) Validate(i interface{}) error {
	return cv.validator.Struct(i)
}

func Start() *echo.Echo {
	log.Info("initializing web server")
	e := echo.New()
	e.HideBanner = true
	e.Validator = &CustomValidator{validator: validator.New()}

	media.SetupRoutes(e)

	log.Infof("starting on port %d", port)
	if err := e.Start(fmt.Sprintf(":%d", port)); err != nil {
		log.Error(err)
	}

	return e
}
