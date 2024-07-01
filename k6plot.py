import json
import matplotlib.pyplot as plt

# Function to load JSON data from a file line by line
def load_json_lines(file_path):
    with open(file_path, 'r') as file:
        lines = file.readlines()
    data = [json.loads(line) for line in lines]
    return data

# Function to extract VUs and request times from the parsed JSON data
def extract_data(json_data):
    vus = []
    request_times = []

    vu_counter = 1  # Start counting VUs from 1
    for entry in json_data:
        if entry['metric'] == 'http_req_duration' and entry['type'] == "Point":
            vus.append(vu_counter)
            request_times.append(entry['data']['value'])
            vu_counter += 1  # Increment VU count for each new request

    return vus, request_times

# Function to plot the data
def plot_data(vus, request_times):
    plt.figure(figsize=(10, 5))
    plt.plot(vus, request_times, marker='o')
    plt.xlabel('Virtual Users (VUs)')
    plt.ylabel('Request Time (ms)')
    plt.title('K6 Load Test: VUs vs Request Time')
    plt.grid(True)
    plt.show()

# Main function to run the script
def main():
    file_path = 'test.json'  # Path to your K6 result file with each line as a separate JSON object
    json_data = load_json_lines(file_path)
    vus, request_times = extract_data(json_data)
    plot_data(vus, request_times)

if __name__ == '__main__':
    main()
