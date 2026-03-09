const Notification = require("../models/Notify");
// create notification
exports.createNotification = async (data) => {

  const notification = new Notification({
    userId: data.userId,
    title: data.title,
    message: data.message,
    type: data.type
  });
  await notification.save();
  return notification;
};

// get notifications by user
exports.getUserNotifications = async (userId) => {
  const notifications = await Notification
    .find({ userId })
    .sort({ createdAt: -1 });
  return notifications;
};


// mark notification as read
exports.markAsRead = async (id) => {

  await Notification.findByIdAndUpdate(
    id,
    { isRead: true }
  );

  return { message: "Notification updated" };
};