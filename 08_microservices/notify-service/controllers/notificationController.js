const notificationService =
    require("../services/notificationService");

// send notification (API)
exports.sendNotification = async (req, res) => {
  try {
      const notification =
          await notificationService.createNotification(req.body);

    res.json(notification);
  } catch (error) {
    console.error(error);
    res.status(500).json({
      message: "Send notification failed"
    });

  }

};


// get notifications by user
exports.getUserNotifications = async (req, res) => {
  try {
    const notifications = await notificationService
      .getUserNotifications(req.params.userId);
    res.json(notifications);
  } catch (error) {
    res.status(500).json(error);
  }
};
// mark notification as read
exports.markAsRead = async (req, res) => {
  try {
    const result = await notificationService
      .markAsRead(req.params.id);
    res.json(result);
  } catch (error) {
    res.status(500).json(error);
  }
};