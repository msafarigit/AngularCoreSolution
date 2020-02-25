import { Injectable } from '@angular/core';
import { loadModules, setDefaultOptions } from 'esri-loader';
import esri = __esri; // Esri TypeScript Types - types/arcgis-js-api.

@Injectable()
export class CoordinateSystemConversionService {

  constructor() { }

  async getPointOnSpatialRefrence(spatialReference, longitude, latitude): Promise<esri.Point> {
    let point = null;

    const [Point, webMercatorUtils] = await loadModules(['esri/geometry/Point', 'esri/geometry/support/webMercatorUtils']);

    if (spatialReference.wkid == null) {
      const coorStr = this.wgsToLambertIran(longitude, latitude);
      const strarr = coorStr.split(',');
      point = new Point(parseFloat(strarr[0]), parseFloat(strarr[1]), spatialReference);
    } else if (spatialReference.wkid.toString().substring(0, 3) === '326') {
      const zone = {};
      const coorStr = this.wgsToUTM(longitude, latitude, zone);
      const strarr = coorStr.split(',');
      point = new Point(parseFloat(strarr[0]), parseFloat(strarr[1]), spatialReference);
    }
    else if (spatialReference.wkid === 102100) {
      point = new Point(longitude, latitude);
      point = webMercatorUtils.geographicToWebMercator(point);
    } else {
      point = new Point(longitude, latitude);
    }
    return point;
  }

  async pointToWGS(pPoint): Promise<esri.Point> {
    let point = null;

    const [Point, webMercatorUtils, SpatialReference] = await loadModules(['esri/geometry/Point', 'esri/geometry/support/webMercatorUtils', 'esri/geometry/SpatialReference']);

    const spatialReference = new SpatialReference(4326);
    if (pPoint.spatialReference.wkid == null) {
      const coorStr = this.lambertIranToWGS(pPoint.X, pPoint.Y);
      const strarr = coorStr.split(';');
      point = new Point(parseFloat(strarr[0]), parseFloat(strarr[1]), spatialReference);
    } else if (pPoint.spatialReference.wkid.toString().substring(0, 3) === '326') {
      const zone = pPoint.spatialReference.wkid.toString().substring(3, 5);
      const coorStr = this.utmToWGS(pPoint.X, pPoint.Y, zone, 'N');
      const strarr = coorStr.split(';');
      point = new Point(parseFloat(strarr[0]), parseFloat(strarr[1]), spatialReference);
    }
    else if (pPoint.spatialReference.wkid === 102100) {
      point = webMercatorUtils.webMercatorToGeographic(pPoint);
    } else {
      point = pPoint;
    }
    return point;
  }

  wgsToLambertIran(lan, fi): string {
    const a = 6378137.0;
    const b = 6356752.314245179;
    const f = 1 / 298.257223563; // (a - b) / a;
    // var e2 = Math.sqrt((Math.pow(a, 2) - Math.pow(b, 2)) / Math.pow(b, 2));
    const e = Math.sqrt(2 * f - Math.pow(f, 2)); // Math.sqrt((Math.pow(a, 2) - Math.pow(b, 2)) / Math.pow(a, 2));

    lan = lan * Math.PI / 180;
    fi = fi * Math.PI / 180;

    // latitudes of the standard parallels
    const fi1 = 30 * Math.PI / 180;
    const fi2 = 36 * Math.PI / 180;

    // central meridian
    const fif = 24 * Math.PI / 180;
    const lanf = 54 * Math.PI / 180;
    const Ef = 2000000;
    const Nf = 40000;

    const m1 = Math.cos(fi1) / Math.sqrt(1 - (Math.pow(e, 2) * Math.pow(Math.sin(fi1), 2)));
    const m2 = Math.cos(fi2) / Math.sqrt(1 - (Math.pow(e, 2) * Math.pow(Math.sin(fi2), 2)));

    const t1 = Math.tan(Math.PI / 4 - (fi1 / 2)) / Math.pow((1 - e * Math.sin(fi1)) / (1 + e * Math.sin(fi1)), e / 2);
    const t2 = Math.tan(Math.PI / 4 - (fi2 / 2)) / Math.pow((1 - e * Math.sin(fi2)) / (1 + e * Math.sin(fi2)), e / 2);
    const tf = Math.tan(Math.PI / 4 - (fif / 2)) / Math.pow((1 - e * Math.sin(fif)) / (1 + e * Math.sin(fif)), e / 2);
    const t = Math.tan(Math.PI / 4 - (fi / 2)) / Math.pow((1 - e * Math.sin(fi)) / (1 + e * Math.sin(fi)), e / 2);

    const n = (Math.log(m1) - Math.log(m2)) / (Math.log(t1) - Math.log(t2));
    const F = m1 / (n * Math.pow(t1, n));
    const r = a * F * Math.pow(t, n);
    const rf = a * F * Math.pow(tf, n);

    const teta = n * (lan - lanf);

    const E = Ef + r * Math.sin(teta);
    const N = Nf + rf - r * Math.cos(teta);

    return (E.toFixed(5).toString() + ',' + N.toFixed(5).toString());
  }

  lambertIranToWGS(E: number, N: number): string {
    const a = 6378137.0;
    // const b = 6356752.314245179;
    const f = 1 / 298.257223563;
    const e = Math.sqrt(2 * f - Math.pow(f, 2));

    // latitudes of the standard parallels
    const fi1 = 30 * Math.PI / 180;
    const fi2 = 36 * Math.PI / 180;

    // central meridian
    const fif = 24 * Math.PI / 180;
    const lanf = 54 * Math.PI / 180;
    const Ef = 2000000;
    const Nf = 40000;

    const m1 = Math.cos(fi1) / Math.sqrt(1 - (Math.pow(e, 2) * Math.pow(Math.sin(fi1), 2)));
    const m2 = Math.cos(fi2) / Math.sqrt(1 - (Math.pow(e, 2) * Math.pow(Math.sin(fi2), 2)));

    const t1 = Math.tan(Math.PI / 4 - (fi1 / 2)) / Math.pow((1 - e * Math.sin(fi1)) / (1 + e * Math.sin(fi1)), e / 2);
    const t2 = Math.tan(Math.PI / 4 - (fi2 / 2)) / Math.pow((1 - e * Math.sin(fi2)) / (1 + e * Math.sin(fi2)), e / 2);
    const tf = Math.tan(Math.PI / 4 - (fif / 2)) / Math.pow((1 - e * Math.sin(fif)) / (1 + e * Math.sin(fif)), e / 2);

    const n = (Math.log(m1) - Math.log(m2)) / (Math.log(t1) - Math.log(t2));
    const F = m1 / (n * Math.pow(t1, n));
    const rf = a * F * Math.pow(tf, n);

    const rp = (n / Math.abs(n)) * Math.sqrt(Math.pow(E - Ef, 2) + Math.pow(rf - (N - Nf), 2));
    const tp = Math.pow(rp / (a * F), (1 / n));
    const tetap = Math.atan((E - Ef) / (rf - (N - Nf)));

    let lan = (tetap / n) + lanf;
    let fi = (Math.PI / 2) - (2 * Math.atan(tp));
    let fit = 0;
    let counter = 0;

    while (Math.abs(fi - fit) > 0.000000001 && counter++ < 6) {
      fit = fi;
      fi = (Math.PI / 2) - (2 * Math.atan(tp * Math.pow((1 - e * Math.sin(fi)) / (1 + e * Math.sin(fi)), e / 2)));
    }

    fi = Math.round((fi * 180 / Math.PI) * 10e8) / 10e8;
    lan = Math.round((lan * 180 / Math.PI) * 10e8) / 10e8;

    return lan.toString().concat(' ; ' + fi.toString());
  }

  wgsToUTM(lambda, pFi, pOutZone): string {
    const a = 6378137.000;
    const b = 6356752.314;
    const f = (a - b) / a;
    const e2 = Math.sqrt((Math.pow(a, 2) - Math.pow(b, 2)) / Math.pow(b, 2));
    const e = Math.sqrt((Math.pow(a, 2) - Math.pow(b, 2)) / Math.pow(a, 2));

    let lan0;
    if (lambda > 0) {
      pOutZone.zone = 30 + Math.ceil(lambda / 6);
      lan0 = Math.floor(lambda / 6) * 6 + 3;
    } else {
      pOutZone.zone = 30 - Math.floor(Math.abs(lambda) / 6);
      lan0 = -Math.floor(Math.abs(lambda) / 6) * 6 - 3;
    }

    let lan = lambda - lan0;
    lan = lan * Math.PI / 180;
    pFi = pFi * Math.PI / 180;
    const N = a / Math.pow(1 - Math.pow(e, 2) * Math.pow(Math.sin(pFi), 2), 0.5);
    const M = a * (1 - Math.pow(e, 2)) / Math.pow((1 - (Math.pow(e, 2) * Math.pow(Math.sin(pFi), 2))), (3 / 2));
    const t = Math.tan(pFi);
    const p = N / M;

    const k0 = 0.9996;

    let term1 = Math.pow(lan, 2) * p * Math.pow(Math.cos(pFi), 2) / 2;
    let term2 = Math.pow(lan, 4) * Math.pow(Math.cos(pFi), 4) *
      (4 * Math.pow(p, 3) * (1 - 6 * Math.pow(t, 2)) + Math.pow(p, 2) * (1 + 24 * Math.pow(t, 2)) - 4 * p * Math.pow(t, 2)) / 24;
    let term3 = Math.pow(lan, 6) * Math.pow(Math.cos(pFi), 6) *
      (61 - 148 * Math.pow(t, 2) + 16 * Math.pow(t, 4)) / 720;

    const Kutm = k0 * (term1 + term2 + term3);

    term1 = Math.pow(lan, 2) * p * Math.pow(Math.cos(pFi), 2) * (p - Math.pow(t, 2)) / 6;
    term2 = Math.pow(lan, 4) * Math.pow(Math.cos(pFi), 4) *
      (4 * Math.pow(p, 3) * (1 - 6 * Math.pow(t, 2)) + Math.pow(p, 2) * (1 + 8 * Math.pow(t, 2)) -
        Math.pow(p, 2) * Math.pow(t, 2) + Math.pow(t, 4)) / 120;
    term3 = Math.pow(lan, 6) * Math.pow(Math.cos(pFi), 6) *
      (61 - 479 * Math.pow(t, 2) + 179 * Math.pow(t, 4) - Math.pow(t, 6)) / 5040;

    const Xutm = 500000 + k0 * lan * N * Math.cos(pFi) * (1 + term1 + term2 + term3);

    const A0 = 1 - 0.25 * Math.pow(e, 2) - 3 / 64 * Math.pow(e, 4) - 5 / 256 * Math.pow(e, 6);
    const A2 = 3 / 8 * (Math.pow(e, 2) + 0.25 * Math.pow(e, 4) + 15 / 128 * Math.pow(e, 6));
    const A4 = 15 / 256 * (Math.pow(e, 4) + 0.75 * Math.pow(e, 6));
    const A6 = 35 / 3072 * Math.pow(e, 6);

    const sfi = a * (A0 * pFi - A2 * Math.sin(2 * pFi) + A4 * Math.sin(4 * pFi) - A6 * Math.sin(6 * pFi));

    term1 = Math.pow(lan, 2) * N * Math.sin(pFi) * Math.cos(pFi) / 2;
    term2 = Math.pow(lan, 4) * N * Math.sin(pFi) * Math.pow(Math.cos(pFi), 3) * (4 * Math.pow(p, 2) + p - Math.pow(t, 2)) / 24;
    term3 = Math.pow(lan, 6) * N * Math.sin(pFi) * Math.pow(Math.cos(pFi), 5) *
      (8 * Math.pow(p, 4) * (11 - 24 * Math.pow(t, 2)) -
        28 * Math.pow(p, 3) * (1 - 6 * Math.pow(t, 2)) +
        Math.pow(p, 2) * (1 - 32 * Math.pow(t, 2)) -
        p * 2 * Math.pow(t, 2) + Math.pow(t, 4));
    const term4 = Math.pow(lan, 8) * N * Math.sin(pFi) * Math.pow(Math.cos(pFi), 7) *
      (1385 - 3111 * Math.pow(t, 2) + 543 * Math.pow(t, 4) - Math.pow(t, 6));

    const Yutm = k0 * (sfi + term1 + term2 + term3 + term4);
    const UTMx = Xutm.toFixed(3);
    const UTMy = Yutm.toFixed(3);
    return (Xutm.toFixed(3).toString() + ',' + Yutm.toFixed(3).toString());
  }

  utmToWGS(X: number, Y: number, zone: number, sn: string) {
    if (sn === 'S') {
      Y = Y - 10000000;
    }
    X = X - 500000;
    const sa = 6378137.000000;
    const sb = 6356752.314245;

    const e = Math.pow(Math.pow(sa, 2) - Math.pow(sb, 2), 0.5) / sa;
    const e2 = Math.pow(Math.pow(sa, 2) - Math.pow(sb, 2), 0.5) / sb;
    const e2cuadrada = Math.pow(e2, 2);
    const c = Math.pow(sa, 2) / sb;

    const S = ((zone * 6) - 183);
    const lat = Y / (6366197.724 * 0.9996);
    const v = (c * 0.9996) / Math.pow(1 + (e2cuadrada * Math.pow(Math.cos(lat), 2)), 0.5);
    const a = X / v;
    const a1 = Math.sin(2 * lat);
    const a2 = a1 * Math.pow(Math.cos(lat), 2);
    const j2 = lat + (a1 / 2);
    const j4 = ((3 * j2) + a2) / 4;
    const j6 = ((5 * j4) + (a2 * Math.pow(Math.cos(lat), 2))) / 3;
    const alfa = (3 / 4) * e2cuadrada;
    const beta = (5 / 3) * Math.pow(alfa, 2);
    const gama = (35 / 27) * Math.pow(alfa, 3);
    const Bm = 0.9996 * c * (lat - alfa * j2 + beta * j4 - gama * j6);
    const b = (Y - Bm) / v;
    const Epsi = ((e2cuadrada * Math.pow(a, 2)) / 2) * Math.pow(Math.cos(lat), 2);
    const Eps = a * (1 - (Epsi / 3));
    const nab = (b * (1 - Epsi)) + lat;
    const senoheps = (Math.exp(Eps) - Math.exp(-Eps)) / 2;
    const Delt = Math.atan(senoheps / Math.cos(nab));
    const TaO = Math.atan(Math.cos(Delt) * Math.tan(nab));
    const longitude = (Delt * (180 / Math.PI)) + S;
    const latitude = (lat +
      (1 + e2cuadrada * Math.pow(Math.cos(lat), 2) - (3 / 2) * e2cuadrada * Math.sin(lat) * Math.cos(lat) * (TaO - lat)) *
      (TaO - lat)) * (180 / Math.PI);

    return longitude.toString().concat(' ; ' + latitude.toString());
  }

  async utmToWebMercator(x: number, y: number, zone: number): Promise<esri.Geometry> {
    const [Point, webMercatorUtils, SpatialReference] = await loadModules(['esri/geometry/Point', 'esri/geometry/support/webMercatorUtils', 'esri/geometry/SpatialReference']);
    const wgsCoor = this.utmToWGS(x, y, zone, 'N').split(';');
    const wgsCoorX = parseFloat(wgsCoor[0]);
    const wgsCoorY = parseFloat(wgsCoor[1]);
    const webMercatorPoint = webMercatorUtils.geographicToWebMercator(new Point(wgsCoorX, wgsCoorY, new SpatialReference(4326)));
    return webMercatorPoint;
  }

  async lambertIranToWebMercator(x, y) {
    const [Point, webMercatorUtils, SpatialReference] = await loadModules(['esri/geometry/Point', 'esri/geometry/support/webMercatorUtils', 'esri/geometry/SpatialReference']);
    const lambertCoor = this.lambertIranToWGS(x, y).split(';');
    const lambertCoorX = parseFloat(lambertCoor[0]);
    const lambertCoorY = parseFloat(lambertCoor[1]);
    const webMercatorPoint = webMercatorUtils.geographicToWebMercator(new Point(lambertCoorX, lambertCoorY, new SpatialReference(4326)));
    return webMercatorPoint;
  }

  async webMercatorToUtm(x, y): Promise<esri.Point> {
    const [Point, webMercatorUtils, SpatialReference] = await loadModules(['esri/geometry/Point', 'esri/geometry/support/webMercatorUtils', 'esri/geometry/SpatialReference']);
    const wgsPoint = webMercatorUtils.webMercatorToGeographic(new Point(x, y, new SpatialReference(102100)));
    const forZone = { zone: 0 };
    const utmCoor = this.wgsToUTM(wgsPoint.x, wgsPoint.y, forZone).split(',');
    return new Point(parseFloat(utmCoor[0]), parseFloat(utmCoor[1]), new SpatialReference(forZone.zone));
  }

  async webMercatorToLambert(x, y): Promise<esri.Point> {
    const [Point, webMercatorUtils, SpatialReference] = await loadModules(['esri/geometry/Point', 'esri/geometry/support/webMercatorUtils', 'esri/geometry/SpatialReference']);
    const wgsPoint = webMercatorUtils.webMercatorToGeographic(new Point(x, y, new SpatialReference(102100)));
    const lambertCoor = this.wgsToLambertIran(wgsPoint.x, wgsPoint.y).split(',');
    return new Point(parseFloat(lambertCoor[0]), parseFloat(lambertCoor[1]));
  }
}
