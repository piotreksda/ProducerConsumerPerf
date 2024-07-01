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

The primary objective of these performance tests is to identify the bottlenecks within the current configuration of the Producer-Consumer architecture. By simulating various load conditions, we aim to determine which component or configuration aspect limits the overall performance.

### Test Setup

1. **RabbitMQ Setup**: RabbitMQ was set up using the default Docker configuration.
2. **Producer Setup**: A .NET application that produces and sends messages to RabbitMQ.
3. **Consumer Setup**: A .NET application that consumes messages from RabbitMQ.
4. **Load Testing with k6**: k6 was configured to simulate different load scenarios to stress test the architecture.
5. **Aspire-dashboard**: Just for local development
6. **MongoDb**: For future tests 

### Testing Scenarios

2. **Peak Load Testing**: Determine the system's performance under maximum load conditions.
4. **Scalability Testing**: Assess the system's ability to handle increasing load by scaling up the number of Producer and Consumer instances.

### Conclusion

The results from these performance tests will help in understanding the current limitations of the Producer-Consumer architecture and guide further improvements. By identifying and addressing the bottlenecks, we aim to enhance the overall efficiency and scalability of the system.



# Performance with single instance 

### Docker

![image](https://github.com/piotreksda/ProducerConsumerPerf/assets/23263384/01c2691f-24b1-484e-b732-084cbb389630)

### Test

Im using k6 with html raport. Script in avilable bellow
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

### Result

k6 raport

![image](https://github.com/piotreksda/ProducerConsumerPerf/assets/23263384/7b3da52c-64dd-4bfb-82cb-a592cc2e0ddd)

docker CPU usage

![image](https://github.com/piotreksda/ProducerConsumerPerf/assets/23263384/3081a496-b06f-4856-b31a-6f3a83eac5fd)


### Conclusion

The container only has access to one core and with the test I achieved its critical performance. The configuration of the docker specifically was defaulted.
