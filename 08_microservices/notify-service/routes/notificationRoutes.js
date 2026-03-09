const express = require("express");
const { sendNotification, getUserNotifications, markAsRead } = require("../controllers/notificationController");
const notificationRouter = express.Router();


notificationRouter.post("/", sendNotification);
notificationRouter.get("/:userId", getUserNotifications);
notificationRouter.put("/read/:id", markAsRead);

module.exports = notificationRouter;