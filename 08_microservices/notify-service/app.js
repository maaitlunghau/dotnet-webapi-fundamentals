const express = require('express');
const connectDB = require('./configs/db');
const notificationRouter = require('./routes/notificationRoutes');
const app = express();

connectDB();

app.use(express.json());

// routes
app.use('/api/notifications', notificationRouter);

const port = 3000;
app.listen(port, () => {
    console.log(`Server is running on port ${port}`);
})