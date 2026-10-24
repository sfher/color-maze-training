import { spawn, spawnSync } from "node:child_process";
import { writeFile } from "node:fs/promises";
import path from "node:path";

const edge = "C:\\Program Files (x86)\\Microsoft\\Edge\\Application\\msedge.exe";
const port = 9333;
const profile = path.join(process.env.TEMP, "codex-edge-cdp-preview");
const targetFile = path.resolve(process.argv[2] ?? "lessons/0001-first-color-grid.html");
const outputFile = path.resolve(process.argv[3] ?? "preview.png");
const targetUrl = "file:///" + targetFile.replaceAll("\\", "/");
const viewportWidth = Number(process.env.PREVIEW_WIDTH ?? 1100);
const viewportHeight = Number(process.env.PREVIEW_HEIGHT ?? 1600);
const anchor = process.env.PREVIEW_ANCHOR ?? "#quiz";

const browser = spawn(edge, [
  "--headless=new",
  "--disable-gpu",
  "--hide-scrollbars",
  "--force-device-scale-factor=1",
  `--remote-debugging-port=${port}`,
  `--user-data-dir=${profile}`,
  `--window-size=${viewportWidth},${viewportHeight}`,
  "about:blank"
], { stdio: "ignore" });

const sleep = (ms) => new Promise((resolve) => setTimeout(resolve, ms));

async function getJson(url) {
  const response = await fetch(url);
  return response.json();
}

async function waitForTarget() {
  for (let attempt = 0; attempt < 40; attempt += 1) {
    try {
      const targets = await getJson(`http://127.0.0.1:${port}/json/list`);
      const page = targets.find((item) => item.type === "page");
      if (page) {
        return page;
      }
    } catch {
      // 浏览器还没启动完成。
    }
    await sleep(250);
  }
  throw new Error("Timed out waiting for Edge debugging target");
}

const target = await waitForTarget();
const socket = new WebSocket(target.webSocketDebuggerUrl);
const pending = new Map();
let nextId = 1;

function send(method, params = {}) {
  const id = nextId;
  nextId += 1;
  socket.send(JSON.stringify({ id, method, params }));
  return new Promise((resolve, reject) => pending.set(id, { resolve, reject }));
}

await new Promise((resolve, reject) => {
  socket.addEventListener("open", resolve, { once: true });
  socket.addEventListener("error", reject, { once: true });
});

socket.addEventListener("message", (event) => {
  const message = JSON.parse(event.data);
  if (message.id && pending.has(message.id)) {
    const { resolve, reject } = pending.get(message.id);
    pending.delete(message.id);
    if (message.error) {
      reject(new Error(message.error.message));
    } else {
      resolve(message.result);
    }
  }
});

try {
  await send("Page.enable");
  await send("Runtime.enable");
  await send("Emulation.setDeviceMetricsOverride", {
    width: viewportWidth,
    height: viewportHeight,
    deviceScaleFactor: 1,
    mobile: viewportWidth < 700
  });
  await send("Page.navigate", { url: targetUrl });
  await sleep(1200);

  const quizResult = await send("Runtime.evaluate", {
    expression: `(() => {
      const quiz = document.querySelector('.quiz[data-answer="1"]');
      if (!quiz) return { error: 'quiz not found' };
      quiz.querySelectorAll('.option')[1].click();
      return {
        feedback: quiz.querySelector('.quiz-feedback').textContent,
        explanationVisible: !quiz.querySelector('.quiz-why').hidden,
        correctCount: quiz.querySelectorAll('.option.correct').length,
        incorrectCount: quiz.querySelectorAll('.option.incorrect').length
      };
    })()`,
    returnByValue: true
  });

  const layout = await send("Runtime.evaluate", {
    expression: `({
      innerWidth: window.innerWidth,
      clientWidth: document.documentElement.clientWidth,
      scrollWidth: document.documentElement.scrollWidth,
      dpr: window.devicePixelRatio,
      pageWidth: document.querySelector('.page').getBoundingClientRect().width
    })`,
    returnByValue: true
  });

  await send("Runtime.evaluate", {
    expression: anchor === "top"
      ? "window.scrollTo(0, 0)"
      : `document.querySelector(${JSON.stringify(anchor)}).scrollIntoView({block: 'start', behavior: 'instant'})`
  });
  await sleep(600);

  const screenshot = await send("Page.captureScreenshot", { format: "png" });
  await writeFile(outputFile, Buffer.from(screenshot.data, "base64"));

  console.log(JSON.stringify({
    url: targetUrl,
    screenshot: outputFile,
    layout: layout.result.value,
    quiz: quizResult.result.value
  }, null, 2));
} finally {
  socket.close();
  spawnSync("taskkill", ["/PID", String(browser.pid), "/T", "/F"], { stdio: "ignore" });
}
