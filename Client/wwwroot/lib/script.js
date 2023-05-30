var offCanvas;
var mupdf = undefined;

var gestureStartScale;
var querySelector;
var cssVar;
var scale;

window.pinchZoomUnload = function pinchZoomUnload() {
    window.removeEventListener('wheel', wheelEvent, { passive: false })

    window.removeEventListener('gesturestart', gestureStart, { passive: false })

    window.removeEventListener('gesturechange', gestureChangeEvent, { passive: false })

    window.removeEventListener('gestureend', gestureEnd, { passive: false })
}

var render = () => {
    DotNet.invokeMethodAsync('DocsWASM.Client', "JSChangeDocumentZoom", scale);
}

var wheelEvent = function (e) {
    if (e.ctrlKey) {
        event.preventDefault();
        scale = getCurrentZoom() - e.deltaY * 0.01;
        render();
    }
};

var gestureChangeEvent = function (e) {
    event.preventDefault();
    scale = gestureStartScale * e.scale;
    render();
}

var gestureStart = function (e) {
    event.preventDefault();
    scale = getCurrentZoom();
    gestureStartScale = scale;
}

var getCurrentZoom = () => {
    return parseFloat(getComputedStyle(document.querySelector(querySelector)).getPropertyValue(cssVar))/100;
};

var gestureEnd = function (e) {
    event.preventDefault();
}

window.pinchZoomLoad = function pinchZoomLoad(_querySelector, _cssVar) {
    gestureStartScale = 0;

    querySelector = _querySelector;
    cssVar = _cssVar;

    window.addEventListener('wheel', wheelEvent, { passive: false })

    window.addEventListener('gesturestart', gestureStart, { passive: false })

    window.addEventListener('gesturechange', gestureChangeEvent, { passive: false })

    window.addEventListener('gestureend', gestureEnd, { passive: false })
};

//window.loadMupdf = async function loadMupdf() {
//    var createMuPdf = require('mupdf-js');
//    mupdf = await createMuPdf.createMuPdf();
//}

window.disposeMupdf = function disposeMupdf() {
    mupdf = undefined;
}

window.blazorFocusElement = function blazorFocusElement(element) {
    if (element instanceof HTMLElement) {
        element.focus({
            preventScroll: true
        });
    }
}

window.freeObjectsUrls = function freeObjectsUrls(urls) {
    for (let i = 0; i < urls.length; i++) {
        URL.revokeObjectURL(urls[i]);
    }
}

window.getObjectsUrls = function getObjectsUrls(datasmimes) {
    const urls = [];
    for (let i = 0; i < datasmimes.length; i++) {
        var blob = new Blob([datasmimes[i].data], { type: datasmimes[i].type });
        urls.push(URL.createObjectURL(blob));
    }
    return urls;
}

//window.convert = async function convert(data) {
//    const svgs = [];
//    const doc = mupdf.load(data);
//    const pages = await mupdf.countPages(doc);
//    for (let i = 1; i <= pages; i++) {
//        svgs.push([mupdf.drawPageAsSVG(doc, i), mupdf.getPageText(doc, i)]);
//    }
//    mupdf.freeDocument(doc);
//    return svgs;
//}

window.offCanvasInit = function offCanvasInit(id) {
    offCanvas = new bootstrap.Offcanvas(document.getElementById(id));
}

window.scrollToElement = function scrollToElement(elementId) {
    var element = document.getElementById(elementId);
    element.scrollIntoView();
}