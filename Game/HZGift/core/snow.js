function makeSnow(canvasId, imagePath) {
    var christmasSnow = new ChristmasSnow(canvasId, imagePath);
    var renderAndUpdateFunc = renderAndUpdate(christmasSnow)
    setInterval(renderAndUpdateFunc, 15);
}

function renderAndUpdate(christmasSnow) {
    return function () {
        christmasSnow.render();
        christmasSnow.update();
    }
}

function ChristmasSnow(canvasId, imagePath) {
    var snowElement = document.getElementById(canvasId);
    this.canvasContext = snowElement.getContext("2d");

    this.width = snowElement.clientWidth;
    this.heigth = snowElement.clientHeight;
    if (imagePath) {
        this.image = initImage(imagePath);
    }
    this.snow = initSnow(this.width, this.heigth);
}

function initImage(imagePath) {
    var image = new Image();
    image.src = imagePath;
    return image;
}

function initSnow(width, height) {
    var minRasius = 1,
        maxRadius = 3,
        minSpeedY = 1,
        maxSpeedY = 2,
        speedX = 0.01,
        minAlpha = 0.5,
        maxAlpha = 1.0,
        minMoveX = 1,
        maxMoveX = 2;
    var snowSettings = new SnowSettings(minRasius, maxRadius, width, height, minSpeedY, maxSpeedY, speedX, minAlpha, maxAlpha, minMoveX, maxMoveX);

    var snow = [];
    var snowNumber = 50;
    for (var i = 0; i < snowNumber; ++i) {
        snow[i] = new Snow(snowSettings);
    }

    return snow;
}

ChristmasSnow.prototype.render = function () {
    // render background image
    if (this.image) {
        this.canvasContext.drawImage(this.image, 0, 0);
    }
    else
    {
        this.canvasContext.clearRect(0, 0, this.width, this.heigth);
    }

    // render snow
    for (var i = 0; i < this.snow.length; ++i) {
        this.snow[i].render(this.canvasContext);
    }
}

ChristmasSnow.prototype.update = function () {
    for (var i = 0; i < this.snow.length; ++i) {
        this.snow[i].update();
    }
}

function SnowSettings(minRadius, maxRadius, maxX, maxY, minSpeedY, maxSpeedY, speedX, minAlpha, maxAlpha, minMoveX, maxMoveX) {
    this.minRadius = minRadius;
    this.maxRadius = maxRadius;
    this.maxX = maxX;
    this.maxY = maxY;
    this.speedX = speedX;
    this.minSpeedY = minSpeedY;
    this.maxSpeedY = maxSpeedY;
    this.minAlpha = minAlpha;
    this.maxAlpha = maxAlpha;
    this.minMoveX = minMoveX;
    this.maxMoveX = maxMoveX;
}

function Snow(snowSettings) {
    this.snowSettings = snowSettings;

    this.radius = randomInRange(snowSettings.minRadius, snowSettings.maxRadius);
    this.initialX = Math.random() * snowSettings.maxX;
    this.y = Math.random() * snowSettings.maxY;
    this.speedY = randomInRange(snowSettings.minSpeedY, snowSettings.maxSpeedY);
    this.speedX = snowSettings.speedX;
    this.alpha = randomInRange(snowSettings.minAlpha, snowSettings.maxAlpha);
    this.angle = Math.random(Math.PI * 2);
    this.x = this.initialX + Math.sin(this.angle);
    this.moveX = randomInRange(snowSettings.minMoveX, snowSettings.maxMoveX);
}

Snow.prototype.render = function (canvasContext) {
    canvasContext.fillStyle = "rgba(255, 255, 255, " + this.alpha + ")";
    canvasContext.beginPath();
    canvasContext.arc(this.x, this.y, this.radius, 0, Math.PI * 2, true);
    canvasContext.closePath();
    canvasContext.fill();
}

Snow.prototype.update = function () {
    this.y += this.speedY;
    if (this.y > this.snowSettings.maxY) {
        this.y -= this.snowSettings.maxY;
    }

    this.angle += this.speedX;
    if (this.angle > Math.PI * 2) {
        this.angle -= Math.PI * 2;
    }

    this.x = this.initialX + this.moveX * Math.sin(this.angle);
}

function randomInRange(min, max) {
    var random = Math.random() * (max - min) + min;
    return random;
}