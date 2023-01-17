const express = require("express");
const app = express();
const path = require("path");
const router = express.Router();

const { simulateSystem } = require("./lib");

const bodyParser = require("body-parser");
const { promiseImpl } = require("ejs");
// parse application/x-www-form-urlencoded
app.use(bodyParser.urlencoded({ extended: false }));

// parse application/json
app.use(bodyParser.json());
app.set("view engine", "ejs");
router.get("/", function (req, res) {
  res.render(path.join(__dirname + "/views/index.ejs"));
});

router.post("/simulate", function (req, res) {
  let result = simulateSystem(req.body.simulationTime, true, 0, 0);
  if (result.stateLogs.length > 10000) delete result.stateLogs;
  res.send(result);
});

router.post("/statistic", function (req, res) {
  let time = 4000;
  let time_result = [];
  while (time <= 150000) {
    let iteration_result = simulateSystem(req.body.simulationTime, false, 0, 0);
    let profit =
      (iteration_result.logs.result.globalStats.income_1 +
        iteration_result.logs.result.globalStats.income_2) / time;
    time_result.push({ time, profit: profit });
    time += 1000;
  }
  let stats = [];
  for (let i = 0; i < 4; i++) {
    for (let j = 0; j < 4; j++) {
      let iteration_res = [];
      for (let m = 0; m < 2; m++) {
        let result = simulateSystem(5000, false, i, j);
        iteration_res.push(
          result.logs.result.globalStats.income_1 +
            result.logs.result.globalStats.income_2
        );
      }
      stats.push({
        z1: i,
        z2: j,
        result: (iteration_res.reduce((prev, curr) => prev+=curr, 0) / 20).toFixed(2),
      });
    }
  }
  stats.sort((a, b) => b.result - a.result)
  res.send({ time_result, stats });
});

//add the router
app.use("/", router);
app.listen(process.env.port || 3000);

console.log("Running at Port 3000");
