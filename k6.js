import http from 'k6/http';
import { sleep, check } from 'k6';
import { htmlReport } from "https://raw.githubusercontent.com/benc-uk/k6-reporter/main/dist/bundle.js";

export const options = {
    discardResponseBodies: true,
    scenarios: {
        contacts: {
            executor: 'ramping-vus',
            startVUs: 0,
            stages: [
                { duration: '20s', target: 350 },
                { duration: '10s', target: 0 },
            ],
            gracefulRampDown: '0s',
        },
    },
};

export default function () {
    const res = http.get('http://localhost/pro/test/');
    check(res, {
        'is status 200': (r) => r.status === 200,
    });
    sleep(0.5);
}

export function handleSummary(data) {
    return {
        "result.html": htmlReport(data)
    };
}
