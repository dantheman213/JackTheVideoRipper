package media

import "github.com/labstack/echo/v4"

func SetupRoutes(e *echo.Echo) {
	e.POST("/v1/media", func(c echo.Context) error {

		return c.NoContent(201)
	})
}
