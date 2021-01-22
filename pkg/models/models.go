package models

type MediaPostRequestSchema struct {
    MediaUrl string `json:"mediaUrl" validate:"required"`
}
