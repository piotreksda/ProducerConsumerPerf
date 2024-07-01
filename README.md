# ProducerConsumerPerf

## Performance Testing Introduction for Producer-Consumer Architecture Using RabbitMQ

This document outlines the approach taken to conduct performance testing on a Producer-Consumer architecture utilizing RabbitMQ. The source code for both the Producer and Consumer components is available in the repository.

### Tools and Setup

- **RabbitMQ**: Used as the messaging broker.
- **Producer (.NET)**: Responsible for producing and sending messages to RabbitMQ.
- **Consumer (.NET)**: Responsible for consuming messages from RabbitMQ.
- **k6**: Utilized for load testing.
- **Machine Specifications**: Tests were conducted on a MacBook Pro M1 with 16GB of RAM.
- **Docker Configuration**: Default configuration was used for setting up RabbitMQ in Docker.

### Objectives

The primary objective of these performance tests is to identify the bottlenecks within the current configuration of the Producer-Consumer architecture. By simulating various load conditions, I aim to determine which component or configuration aspect limits the overall performance.

### Test Setup

1. **RabbitMQ Setup**: RabbitMQ was set up using the default Docker configuration.
2. **Producer Setup**: A .NET application that produces and sends messages to RabbitMQ.
3. **Consumer Setup**: A .NET application that consumes messages from RabbitMQ.
4. **Load Testing with k6**: k6 was configured to simulate different load scenarios to stress test the architecture.
5. **Aspire-dashboard**: Just for local development.
6. **MongoDB**: For future tests.

### Testing Scenarios

1. **Peak Load Testing**: Determine the system's performance under maximum load conditions.
2. **Scalability Testing**: Assess the system's ability to handle increasing load by scaling up the number of Producer and Consumer instances.

### Conclusion

The results from these performance tests will help in understanding the current limitations of the Producer-Consumer architecture and guide further improvements. By identifying and addressing the bottlenecks, I aim to enhance the overall efficiency and scalability of the system.

# Test Config
### Test

I'm using k6 with HTML report. Script is available below.

```js
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
```


# Performance with single instance 

### Docker

![image](https://github.com/piotreksda/ProducerConsumerPerf/assets/23263384/01c2691f-24b1-484e-b732-084cbb389630)

### Result

k6 raport

![image](https://github.com/piotreksda/ProducerConsumerPerf/assets/23263384/7b3da52c-64dd-4bfb-82cb-a592cc2e0ddd)

* there is bug of time should be 08:09 

docker CPU usage

![image](https://github.com/piotreksda/ProducerConsumerPerf/assets/23263384/3081a496-b06f-4856-b31a-6f3a83eac5fd)


### Conclusion

The container only has access to one core, and with the test I achieved its critical performance. The configuration of Docker specifically was defaulted.

# Performance with two instances of Producer and two instances of Consumer

### Docker

![image](https://github.com/piotreksda/ProducerConsumerPerf/assets/23263384/1d72d1a2-dccd-4837-ba7b-72a177c47783)

### Result

k6 raport

![image](https://github.com/piotreksda/ProducerConsumerPerf/assets/23263384/7e24f673-ae4e-48bd-aa3b-f4c722e2131c)


docker CPU usage

Producer-1

![image](https://github.com/piotreksda/ProducerConsumerPerf/assets/23263384/b7073e29-6451-43fd-97f8-243bc0bcc1bd)

Producer-2

![image](https://github.com/piotreksda/ProducerConsumerPerf/assets/23263384/207e6acd-4174-483f-9d73-6317f68d6130)


# Summary of test without and with load balancer

In the first test without the load balancer, the critical performance of a single container was reached with the default settings, resulting in a much longer response time than in the first test. The results clearly show that the load balancer and horizontal scaling produce measurable results. Below is a table with the results.

| Scenario       | Average | Maximum | Median | Minimum | 90th Percentile | 95th Percentile |
|----------------|---------|---------|--------|---------|-----------------|-----------------|
| **One Instance** | 84.81   | 482.71  | 41.13  | 3.68    | 217.04          | 339.57          |
| **Load Balance** | 3.60    | 346.28  | 2.84   | 1.08    | 5.29            | 6.74            |

* Times in ms
