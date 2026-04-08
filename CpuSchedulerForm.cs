using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace CpuScheduler
{
    /// <summary>
    /// Main form for demonstrating CPU scheduling algorithms.
    /// </summary>
    public partial class CpuSchedulerForm : Form
    {
        private DataTable processTable;
        private Random random = new Random();
        private bool isDarkMode = true;

        private const int MIN_PROCESS_COUNT = 1;
        private const int MAX_PROCESS_COUNT = 100;
        private const int DEFAULT_PROCESS_COUNT = 3;

private void DisplayResults(List<SchedulingResult> results, string algorithmName)
{
    if (listView1.Columns.Count == 0)
    {
        listView1.View = View.Details; 
        listView1.Columns.Add("Process", 70);
        listView1.Columns.Add("Arrival", 70);
        listView1.Columns.Add("Burst", 70);
        listView1.Columns.Add("Finish", 70);
        listView1.Columns.Add("Turnaround", 90);
        listView1.Columns.Add("Wait", 70);
        listView1.Columns.Add("Response", 90);
    }

    listView1.Items.Clear();

    double totalWait = 0;
    double totalTurnaround = 0;
    int maxFinishTime = 0;

    foreach (var res in results)
    {
        ListViewItem item = new ListViewItem(res.ProcessID);
        item.SubItems.Add(res.ArrivalTime.ToString());
        item.SubItems.Add(res.BurstTime.ToString());
        item.SubItems.Add(res.FinishTime.ToString());
        item.SubItems.Add(res.TurnaroundTime.ToString());
        item.SubItems.Add(res.WaitingTime.ToString());
        item.SubItems.Add(res.ResponseTime.ToString());
        item.SubItems.Add(res.WaitingTime.ToString());
        item.SubItems.Add(res.ResponseTime.ToString());

        listView1.Items.Add(item);

        totalWait += res.WaitingTime;
        totalTurnaround += res.TurnaroundTime;
        
        if (res.FinishTime > maxFinishTime) maxFinishTime = res.FinishTime;
    }

    // Calculations
    double awt = results.Count > 0 ? totalWait / results.Count : 0;
    double att = results.Count > 0 ? totalTurnaround / results.Count : 0;
    double throughput = results.Count > 0 ? (double)results.Count / maxFinishTime : 0;

    // --- UI Update (The "Panel Swap") ---
    welcomePanel.Visible = false;  
    aboutPanel.Visible = false;    
    schedulerPanel.Visible = false;
    
    resultsPanel.Visible = true;
    resultsPanel.BringToFront();
    listView1.BringToFront(); 

    // Final Summary Popup
    MessageBox.Show($"{algorithmName} Results:\n\n" +
                    $"Average Waiting Time: {awt:F2}\n" +
                    $"Average Turnaround Time: {att:F2}\n" +
                    $"Throughput: {throughput:F2} processes/unit", 
                    "OwlTech Scheduler Analysis");
}
        private List<ProcessData> GetProcessesFromGrid()
{
    List<ProcessData> processes = new List<ProcessData>();

    try
    {
        foreach (DataGridViewRow row in processDataGrid.Rows)
        {
            // Skip the empty "new row" at the bottom of the grid
            if (row.IsNewRow) continue;

            // Ensure the cells aren't null before converting
            if (row.Cells[0].Value != null && row.Cells[1].Value != null && 
    row.Cells[2].Value != null && row.Cells[3].Value != null)
            {
               processes.Add(new ProcessData
                {
                     ProcessID = row.Cells[0].Value.ToString(),
                     BurstTime = Convert.ToInt32(row.Cells[1].Value),    // Match Column 1
                     Priority = Convert.ToInt32(row.Cells[2].Value),     // Match Column 2
                    ArrivalTime = Convert.ToInt32(row.Cells[3].Value)   // Match Column 3
                });
            }
        }
    }
    catch (Exception ex)
    {
        MessageBox.Show("Error reading grid data. Please ensure all Arrival and Burst times are numbers.\n" + ex.Message);
    }

    return processes;
}

        public CpuSchedulerForm()
        {
            InitializeComponent();
            InitializeProcessTable();
        }

        private void WelcomeButton_Click(object sender, EventArgs e)
        {
            ShowPanel(welcomePanel);
            sidePanel.Height = btnWelcome.Height;
            sidePanel.Top = btnWelcome.Top;
        }
        

        private void DashBoardButton_Click(object sender, EventArgs e)
        {
            ShowPanel(resultsPanel);
            sidePanel.Height = btnDashBoard.Height;
            sidePanel.Top = btnDashBoard.Top;
        }

        private void CpuSchedulerButton_Click(object sender, EventArgs e)
        {
            ShowPanel(schedulerPanel);
            sidePanel.Height = btnCpuScheduler.Height;
            sidePanel.Top = btnCpuScheduler.Top;
        }

        private void AboutButton_Click(object sender, EventArgs e)
        {
            ShowPanel(aboutPanel);
            sidePanel.Height = btnAbout.Height;
            sidePanel.Top = btnAbout.Top;
        }

        private void DarkModeToggle_Click(object sender, EventArgs e)
        {
            isDarkMode = !isDarkMode;
            ApplyTheme();
        }
        

        private void ShowPanel(Panel panelToShow)
        {
            welcomePanel.Visible = false;
            schedulerPanel.Visible = false;
            resultsPanel.Visible = false;
            aboutPanel.Visible = false;
            panelToShow.Visible = true;
            panelToShow.BringToFront();
        }

        private void InitializeWelcomeContent()
        {
            welcomeTextBox.Text = @"Welcome to CPU Scheduler Simulator

This educational tool helps CS 3502 students learn and experiment with CPU scheduling algorithms used in operating systems.

GETTING STARTED

Navigate using the sidebar buttons on the left:

🏠 WELCOME
This introduction page explaining the simulator and navigation.

⚙️ SCHEDULER
The main interface where you can:
• Enter the number of processes to simulate
• Choose from 6 scheduling algorithms:
  - FCFS (First Come, First Serve)
  - SJF (Shortest Job First)
  - Priority Scheduling
  - Round Robin
  - SRTF (Shortest Remaining Time First)  [NEW]
  - HRRN (Highest Response Ratio Next)   [NEW]
• Run simulations and see immediate feedback

📊 RESULTS
View detailed results from your last algorithm execution:
• Process execution details
• Algorithm-specific information
• Summary statistics including CPU Utilization and Throughput
Results persist until you run a new simulation.

📚 ABOUT
Learn about the algorithms including the two new additions.

HOW TO USE
1. Click 'Scheduler' to start
2. Enter number of processes (try 3-5 for learning)
3. Click an algorithm button to run simulation
4. View results in the 'Results' section
5. Experiment with different algorithms and process counts

Ready to start? Click 'Scheduler' to begin!";
        }

        private void InitializeAboutContent()
        {
            aboutTextBox.Text = @"CPU Scheduling Algorithms

This simulator demonstrates six CPU scheduling algorithms:

FIRST COME, FIRST SERVE (FCFS)
• Non-preemptive algorithm
• Processes are executed in the order they arrive
• Simple to implement but can lead to convoy effect
• Good for batch systems with long processes

SHORTEST JOB FIRST (SJF)
• Non-preemptive algorithm
• Selects process with shortest burst time first
• Optimal for minimizing average waiting time
• Requires knowledge of process execution times

PRIORITY SCHEDULING
• Non-preemptive; higher priority number = higher priority
• CPU allocated to highest priority process
• May cause starvation of low-priority processes

ROUND ROBIN (RR)
• Preemptive algorithm using time quantum
• Each process gets equal CPU time slices
• Good for time-sharing systems
• Performance depends on quantum size

SHORTEST REMAINING TIME FIRST (SRTF) [NEW]
• Preemptive version of SJF
• Preempts current process when a shorter job arrives
• Optimal average waiting time among all algorithms
• High context-switch overhead; can starve long processes
• Uses a min-heap priority queue on remaining burst time

HIGHEST RESPONSE RATIO NEXT (HRRN) [NEW]
• Non-preemptive algorithm
• Response Ratio = (Waiting Time + Burst Time) / Burst Time
• Selects process with the highest response ratio at each decision
• Balances SJF efficiency with fairness (prevents starvation)
• Great compromise between FCFS and SJF

Performance Metrics Displayed:
• Average Waiting Time (AWT)
• Average Turnaround Time (ATT)
• CPU Utilization (%)
• Throughput (processes / time unit)
• Response Time (first execution - arrival)";
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Data structures
        // ─────────────────────────────────────────────────────────────────────

        public class ProcessData
        {
            public string ProcessID { get; set; }
            public int BurstTime { get; set; }
            public int Priority { get; set; }
            public int ArrivalTime { get; set; }
        }

        public class SchedulingResult
        {
            public string ProcessID { get; set; }
            public int ArrivalTime { get; set; }
            public int BurstTime { get; set; }
            public int StartTime { get; set; }
            public int FinishTime { get; set; }
            public int WaitingTime { get; set; }
            public int TurnaroundTime { get; set; }
            public int ResponseTime { get; set; }
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Helper methods
        // ─────────────────────────────────────────────────────────────────────

        public List<ProcessData> GetProcessDataFromGrid()
        {
            var processList = new List<ProcessData>();
            foreach (DataRow row in processTable.Rows)
            {
                processList.Add(new ProcessData
                {
                    ProcessID = row["Process ID"].ToString(),
                    BurstTime = Convert.ToInt32(row["Burst Time"]),
                    Priority = Convert.ToInt32(row["Priority"]),
                    ArrivalTime = Convert.ToInt32(row["Arrival Time"])
                });
            }
            return processList;
        }
        

        private bool IsValidProcessCount(string input, out int processCount)
        {
            if (int.TryParse(input, out processCount))
                return processCount >= MIN_PROCESS_COUNT && processCount <= MAX_PROCESS_COUNT;
            processCount = 0;
            return false;
        }
        

        // ─────────────────────────────────────────────────────────────────────
        //  Algorithm implementations
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>First Come First Serve – non-preemptive, sort by arrival time.</summary>
        private List<SchedulingResult> RunFCFSAlgorithm(List<ProcessData> processes)
        {
            var results = new List<SchedulingResult>();
            int currentTime = 0;

            foreach (var process in processes.OrderBy(p => p.ArrivalTime))
            {
                int startTime = Math.Max(currentTime, process.ArrivalTime);
                int finishTime = startTime + process.BurstTime;

                results.Add(new SchedulingResult
                {
                    ProcessID = process.ProcessID,
                    ArrivalTime = process.ArrivalTime,
                    BurstTime = process.BurstTime,
                    StartTime = startTime,
                    FinishTime = finishTime,
                    WaitingTime = startTime - process.ArrivalTime,
                    TurnaroundTime = finishTime - process.ArrivalTime,
                    ResponseTime = startTime - process.ArrivalTime
                });

                currentTime = finishTime;
            }

            return results;
        }

        /// <summary>Shortest Job First – non-preemptive, select minimum burst time among arrived processes.</summary>
        private List<SchedulingResult> RunSJFAlgorithm(List<ProcessData> processes)
        {
            var results = new List<SchedulingResult>();
            int currentTime = 0;
            var remaining = processes.ToList();

            while (remaining.Count > 0)
            {
                var available = remaining.Where(p => p.ArrivalTime <= currentTime).ToList();
                if (available.Count == 0)
                {
                    currentTime = remaining.Min(p => p.ArrivalTime);
                    continue;
                }

                var next = available.OrderBy(p => p.BurstTime).ThenBy(p => p.ArrivalTime).First();
                int startTime = Math.Max(currentTime, next.ArrivalTime);
                int finishTime = startTime + next.BurstTime;

                results.Add(new SchedulingResult
                {
                    ProcessID = next.ProcessID,
                    ArrivalTime = next.ArrivalTime,
                    BurstTime = next.BurstTime,
                    StartTime = startTime,
                    FinishTime = finishTime,
                    WaitingTime = startTime - next.ArrivalTime,
                    TurnaroundTime = finishTime - next.ArrivalTime,
                    ResponseTime = startTime - next.ArrivalTime
                });

                currentTime = finishTime;
                remaining.Remove(next);
            }

            return results.OrderBy(r => r.StartTime).ToList();
        }

        /// <summary>Priority Scheduling – non-preemptive; higher Priority number = higher priority.</summary>
        private List<SchedulingResult> RunPriorityAlgorithm(List<ProcessData> processes)
        {
            var results = new List<SchedulingResult>();
            int currentTime = 0;
            var remaining = processes.ToList();

            while (remaining.Count > 0)
            {
                var available = remaining.Where(p => p.ArrivalTime <= currentTime).ToList();
                if (available.Count == 0)
                {
                    currentTime = remaining.Min(p => p.ArrivalTime);
                    continue;
                }

                var next = available.OrderByDescending(p => p.Priority).ThenBy(p => p.ArrivalTime).First();
                int startTime = Math.Max(currentTime, next.ArrivalTime);
                int finishTime = startTime + next.BurstTime;

                results.Add(new SchedulingResult
                {
                    ProcessID = next.ProcessID,
                    ArrivalTime = next.ArrivalTime,
                    BurstTime = next.BurstTime,
                    StartTime = startTime,
                    FinishTime = finishTime,
                    WaitingTime = startTime - next.ArrivalTime,
                    TurnaroundTime = finishTime - next.ArrivalTime,
                    ResponseTime = startTime - next.ArrivalTime
                });

                currentTime = finishTime;
                remaining.Remove(next);
            }

            return results.OrderBy(r => r.StartTime).ToList();
        }

        /// <summary>Round Robin – preemptive, each process gets a time quantum before switching.</summary>
        private List<SchedulingResult> RunRoundRobinAlgorithm(List<ProcessData> processes, int quantumTime = 4)
        {
            var results = new List<SchedulingResult>();
            var processQueue = new Queue<ProcessData>();
            var processResults = new Dictionary<string, SchedulingResult>();
            var remainingBurst = new Dictionary<string, int>();
            int currentTime = 0;

            foreach (var p in processes)
            {
                remainingBurst[p.ProcessID] = p.BurstTime;
                processResults[p.ProcessID] = new SchedulingResult
                {
                    ProcessID = p.ProcessID,
                    ArrivalTime = p.ArrivalTime,
                    BurstTime = p.BurstTime,
                    StartTime = -1,
                    ResponseTime = -1
                };
            }

            foreach (var p in processes.Where(p => p.ArrivalTime <= currentTime).OrderBy(p => p.ArrivalTime))
                processQueue.Enqueue(p);

            var notInQueue = processes.Where(p => p.ArrivalTime > currentTime).OrderBy(p => p.ArrivalTime).ToList();

            while (processQueue.Count > 0 || notInQueue.Count > 0)
            {
                while (notInQueue.Count > 0 && notInQueue[0].ArrivalTime <= currentTime)
                {
                    processQueue.Enqueue(notInQueue[0]);
                    notInQueue.RemoveAt(0);
                }

                if (processQueue.Count == 0)
                {
                    currentTime = notInQueue[0].ArrivalTime;
                    continue;
                }

                var current = processQueue.Dequeue();
                var res = processResults[current.ProcessID];

                if (res.StartTime == -1)
                {
                    res.StartTime = currentTime;
                    res.ResponseTime = currentTime - current.ArrivalTime;
                }

                int execTime = Math.Min(quantumTime, remainingBurst[current.ProcessID]);
                currentTime += execTime;
                remainingBurst[current.ProcessID] -= execTime;

                while (notInQueue.Count > 0 && notInQueue[0].ArrivalTime <= currentTime)
                {
                    processQueue.Enqueue(notInQueue[0]);
                    notInQueue.RemoveAt(0);
                }

                if (remainingBurst[current.ProcessID] == 0)
                {
                    res.FinishTime = currentTime;
                    res.TurnaroundTime = res.FinishTime - res.ArrivalTime;
                    res.WaitingTime = res.TurnaroundTime - res.BurstTime;
                }
                else
                {
                    processQueue.Enqueue(current);
                }
            }

            return processResults.Values.OrderBy(r => r.FinishTime).ToList();
        }

        // ─────────────────────────────────────────────────────────────────────
        //  NEW ALGORITHM 1: Shortest Remaining Time First (SRTF)
        //  Preemptive SJF – always run the process with the least remaining time.
        //  At each time unit, check if a newly arrived process has a shorter
        //  remaining burst than the running process and preempt if so.
        // ─────────────────────────────────────────────────────────────────────
        private List<SchedulingResult> RunSRTFAlgorithm(List<ProcessData> processes)
        {
            int n = processes.Count;
            var remaining = new Dictionary<string, int>();
            var firstRun = new Dictionary<string, bool>();
            var resultMap = new Dictionary<string, SchedulingResult>();

            foreach (var p in processes)
            {
                remaining[p.ProcessID] = p.BurstTime;
                firstRun[p.ProcessID] = true;
                resultMap[p.ProcessID] = new SchedulingResult
                {
                    ProcessID = p.ProcessID,
                    ArrivalTime = p.ArrivalTime,
                    BurstTime = p.BurstTime,
                    StartTime = -1,
                    ResponseTime = -1
                };
            }

            int complete = 0;
            int t = 0;
            int maxTime = processes.Sum(p => p.BurstTime) + processes.Max(p => p.ArrivalTime) + 1;

            while (complete < n && t <= maxTime)
            {
                // Eligible: arrived and not yet finished
                var eligible = processes
                    .Where(p => p.ArrivalTime <= t && remaining[p.ProcessID] > 0)
                    .OrderBy(p => remaining[p.ProcessID])
                    .ThenBy(p => p.ArrivalTime)
                    .FirstOrDefault();

                if (eligible == null)
                {
                    t++;
                    continue;
                }

                var res = resultMap[eligible.ProcessID];

                if (firstRun[eligible.ProcessID])
                {
                    res.StartTime = t;
                    res.ResponseTime = t - eligible.ArrivalTime;
                    firstRun[eligible.ProcessID] = false;
                }

                remaining[eligible.ProcessID]--;
                t++;

                if (remaining[eligible.ProcessID] == 0)
                {
                    res.FinishTime = t;
                    res.TurnaroundTime = res.FinishTime - eligible.ArrivalTime;
                    res.WaitingTime = res.TurnaroundTime - eligible.BurstTime;
                    complete++;
                }
                
            }

            return resultMap.Values.OrderBy(r => r.FinishTime).ToList();
        }

        // ─────────────────────────────────────────────────────────────────────
        //  NEW ALGORITHM 2: Highest Response Ratio Next (HRRN)
        //  Non-preemptive. At each scheduling point choose the process with:
        //      Response Ratio = (WaitingTime + BurstTime) / BurstTime
        //  This prevents starvation because waiting time grows over time,
        //  eventually boosting low-priority / long processes to the front.
        // ─────────────────────────────────────────────────────────────────────
        private List<SchedulingResult> RunHRRNAlgorithm(List<ProcessData> processes)
        {
            var results = new List<SchedulingResult>();
            int currentTime = 0;
            var remaining = processes.ToList();

            while (remaining.Count > 0)
            {
                var available = remaining.Where(p => p.ArrivalTime <= currentTime).ToList();

                if (available.Count == 0)
                {
                    currentTime = remaining.Min(p => p.ArrivalTime);
                    continue;
                }

                // Calculate response ratio for each available process
                var next = available
                    .OrderByDescending(p =>
                    {
                        double waitingTime = currentTime - p.ArrivalTime;
                        return (waitingTime + p.BurstTime) / (double)p.BurstTime;
                    })
                    .ThenBy(p => p.ArrivalTime)
                    .First();

                int startTime = Math.Max(currentTime, next.ArrivalTime);
                int finishTime = startTime + next.BurstTime;

                results.Add(new SchedulingResult
                {
                    ProcessID = next.ProcessID,
                    ArrivalTime = next.ArrivalTime,
                    BurstTime = next.BurstTime,
                    StartTime = startTime,
                    FinishTime = finishTime,
                    WaitingTime = startTime - next.ArrivalTime,
                    TurnaroundTime = finishTime - next.ArrivalTime,
                    ResponseTime = startTime - next.ArrivalTime
                });

                currentTime = finishTime;
                remaining.Remove(next);
            }

            return results.OrderBy(r => r.StartTime).ToList();
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Results display
        // ─────────────────────────────────────────────────────────────────────

        private void DisplaySchedulingResults(List<SchedulingResult> results, string algorithmName)
        {
            listView1.Clear();
            listView1.View = View.Details;

            listView1.Columns.Add("Process ID", 90, HorizontalAlignment.Center);
            listView1.Columns.Add("Arrival", 70, HorizontalAlignment.Center);
            listView1.Columns.Add("Burst", 70, HorizontalAlignment.Center);
            listView1.Columns.Add("Start", 70, HorizontalAlignment.Center);
            listView1.Columns.Add("Finish", 70, HorizontalAlignment.Center);
            listView1.Columns.Add("Waiting", 70, HorizontalAlignment.Center);
            listView1.Columns.Add("Turnaround", 90, HorizontalAlignment.Center);
            listView1.Columns.Add("Response", 80, HorizontalAlignment.Center);

            foreach (var result in results)
            {
                var item = new ListViewItem(result.ProcessID);
                item.SubItems.Add(result.ArrivalTime.ToString());
                item.SubItems.Add(result.BurstTime.ToString());
                item.SubItems.Add(result.StartTime.ToString());
                item.SubItems.Add(result.FinishTime.ToString());
                item.SubItems.Add(result.WaitingTime.ToString());
                item.SubItems.Add(result.TurnaroundTime.ToString());
                item.SubItems.Add(result.ResponseTime.ToString());
                listView1.Items.Add(item);
            }

            // ── Performance Metrics ──────────────────────────────────────────
            double awt = results.Average(r => r.WaitingTime);
            double att = results.Average(r => r.TurnaroundTime);
            double art = results.Average(r => r.ResponseTime);
            int totalBurst = results.Sum(r => r.BurstTime);
            int totalTime = results.Max(r => r.FinishTime) - results.Min(r => r.ArrivalTime);
            double cpuUtil = totalTime > 0 ? (totalBurst / (double)totalTime) * 100.0 : 100.0;
            double throughput = totalTime > 0 ? results.Count / (double)totalTime : 0;

            // Blank separator
            listView1.Items.Add(new ListViewItem(""));

            // Summary header
            var headerItem = new ListViewItem("── METRICS ──");
            headerItem.SubItems.Add(algorithmName);
            headerItem.SubItems.Add($"{results.Count} processes");
            headerItem.SubItems.Add(""); headerItem.SubItems.Add("");
            headerItem.SubItems.Add(""); headerItem.SubItems.Add("");
            headerItem.SubItems.Add("");
            headerItem.Font = new Font(listView1.Font, FontStyle.Bold);
            listView1.Items.Add(headerItem);

            AddMetricRow("Avg Waiting Time (AWT)", $"{awt:F2} units");
            AddMetricRow("Avg Turnaround Time (ATT)", $"{att:F2} units");
            AddMetricRow("Avg Response Time (ART)", $"{art:F2} units");
            AddMetricRow("CPU Utilization", $"{cpuUtil:F1} %");
            AddMetricRow("Throughput", $"{throughput:F4} proc/unit");
        }

        private void AddMetricRow(string label, string value)
        {
            var item = new ListViewItem(label);
            item.SubItems.Add(value);
            // pad remaining columns
            for (int i = 0; i < 6; i++) item.SubItems.Add("");
            listView1.Items.Add(item);
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Button click handlers for new algorithms
        // ─────────────────────────────────────────────────────────────────────

        private void SRTFButton_Click(object sender, EventArgs e)
{
    var inputProcesses = GetProcessesFromGrid();
    if (inputProcesses.Count == 0) return;

    int n = inputProcesses.Count;
    int[] remainingTime = inputProcesses.Select(p => p.BurstTime).ToArray();
    bool[] isCompleted = new bool[n];
    var results = new List<SchedulingResult>();
    
    foreach (var p in inputProcesses) {
        results.Add(new SchedulingResult { 
            ProcessID = p.ProcessID, ArrivalTime = p.ArrivalTime, BurstTime = p.BurstTime, StartTime = -1 
        });
    }

    int currentTime = 0, completed = 0;
    while (completed != n)
    {
        int shortest = -1;
        int minRemaining = int.MaxValue;

        for (int i = 0; i < n; i++) {
            if (inputProcesses[i].ArrivalTime <= currentTime && !isCompleted[i]) {
                if (remainingTime[i] < minRemaining) {
                    minRemaining = remainingTime[i];
                    shortest = i;
                }
                // Tie-breaker: earlier arrival time
                else if (remainingTime[i] == minRemaining && inputProcesses[i].ArrivalTime < inputProcesses[shortest].ArrivalTime) {
                    shortest = i;
                }
            }
        }

        if (shortest == -1) { currentTime++; continue; }

        // Set StartTime and ResponseTime only on first execution
        if (results[shortest].StartTime == -1) {
            results[shortest].StartTime = currentTime;
            results[shortest].ResponseTime = results[shortest].StartTime - results[shortest].ArrivalTime;
        }

        remainingTime[shortest]--;
        currentTime++; // Increment time after work is done

        if (remainingTime[shortest] == 0) {
            results[shortest].FinishTime = currentTime;
            results[shortest].TurnaroundTime = results[shortest].FinishTime - results[shortest].ArrivalTime;
            results[shortest].WaitingTime = results[shortest].TurnaroundTime - results[shortest].BurstTime;
            isCompleted[shortest] = true;
            completed++;
        }
    }

    DisplaySchedulingResults(results, "SRTF - Shortest Remaining Time First");
    ShowPanel(resultsPanel);
}

        private void HRRNButton_Click(object sender, EventArgs e)
{
    var inputProcesses = GetProcessesFromGrid();
    if (inputProcesses.Count == 0) return;

    int n = inputProcesses.Count;
    var results = new List<SchedulingResult>();
    bool[] isCompleted = new bool[n];
    int currentTime = 0, completed = 0;

    while (completed < n)
    {
        int bestProcess = -1;
        double highestRatio = -1.0;

        for (int i = 0; i < n; i++) {
            if (inputProcesses[i].ArrivalTime <= currentTime && !isCompleted[i]) {
                int waitTime = currentTime - inputProcesses[i].ArrivalTime;
                double ratio = (double)(waitTime + inputProcesses[i].BurstTime) / inputProcesses[i].BurstTime;
                
                if (ratio > highestRatio) {
                    highestRatio = ratio;
                    bestProcess = i;
                }
                // Tie-breaker: shorter burst time
                else if (Math.Abs(ratio - highestRatio) < 0.001 && inputProcesses[i].BurstTime < inputProcesses[bestProcess].BurstTime) {
                    bestProcess = i;
                }
            }
        }

        if (bestProcess == -1) {
            currentTime++; 
        }
        else {
            var p = inputProcesses[bestProcess];
            results.Add(new SchedulingResult {
                ProcessID = p.ProcessID, ArrivalTime = p.ArrivalTime, BurstTime = p.BurstTime,
                StartTime = currentTime, 
                ResponseTime = currentTime - p.ArrivalTime,
                FinishTime = currentTime + p.BurstTime,
                TurnaroundTime = (currentTime + p.BurstTime) - p.ArrivalTime,
                WaitingTime = currentTime - p.ArrivalTime
            });
            currentTime += p.BurstTime;
            isCompleted[bestProcess] = true;
            completed++;
        }
    }

    DisplaySchedulingResults(results, "HRRN - Highest Response Ratio Next");
    ShowPanel(resultsPanel);
}

        // ─────────────────────────────────────────────────────────────────────
        //  Existing algorithm button click handlers
        // ─────────────────────────────────────────────────────────────────────

        private void FirstComeFirstServeButton_Click(object sender, EventArgs e)
        {
            var processData = GetProcessesFromGrid();
            if (processData.Count > 0)
            {
                var results = RunFCFSAlgorithm(processData);
                DisplaySchedulingResults(results, "FCFS - First Come First Serve");
                ShowPanel(resultsPanel);
                sidePanel.Height = btnDashBoard.Height;
                sidePanel.Top = btnDashBoard.Top;
            }
            else
            {
                MessageBox.Show("Please set process count and ensure the data grid has process data.",
                    "No Process Data", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtProcess.Focus();
            }
        }

        private void ShortestJobFirstButton_Click(object sender, EventArgs e)
        {
            var processData = GetProcessesFromGrid();
            if (processData.Count > 0)
            {
                var results = RunSJFAlgorithm(processData);
                DisplaySchedulingResults(results, "SJF - Shortest Job First");
                ShowPanel(resultsPanel);
                sidePanel.Height = btnDashBoard.Height;
                sidePanel.Top = btnDashBoard.Top;
            }
            else
            {
                MessageBox.Show("Please set process count and ensure the data grid has process data.",
                    "No Process Data", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtProcess.Focus();
            }
        }

        private void PriorityButton_Click(object sender, EventArgs e)
        {
            var processData = GetProcessesFromGrid();
            if (processData.Count > 0)
            {
                var results = RunPriorityAlgorithm(processData);
                DisplaySchedulingResults(results, "Priority Scheduling (Higher # = Higher Priority)");
                ShowPanel(resultsPanel);
                sidePanel.Height = btnDashBoard.Height;
                sidePanel.Top = btnDashBoard.Top;
            }
            else
            {
                MessageBox.Show("Please set process count and ensure the data grid has process data.",
                    "No Process Data", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtProcess.Focus();
            }
        }

        private void RoundRobinButton_Click(object sender, EventArgs e)
        {
            var processData = GetProcessesFromGrid();
            if (processData.Count > 0)
            {
                string quantumInput = Microsoft.VisualBasic.Interaction.InputBox(
                    "Enter quantum time for Round Robin scheduling:",
                    "Quantum Time", "4");

                if (int.TryParse(quantumInput, out int quantumTime) && quantumTime > 0)
                {
                    var results = RunRoundRobinAlgorithm(processData, quantumTime);
                    DisplaySchedulingResults(results, $"Round Robin (Quantum = {quantumTime})");
                    ShowPanel(resultsPanel);
                    sidePanel.Height = btnDashBoard.Height;
                    sidePanel.Top = btnDashBoard.Top;
                }
                else
                {
                    MessageBox.Show("Please enter a valid quantum time (positive integer).",
                        "Invalid Quantum Time", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("Please set process count and ensure the data grid has process data.",
                    "No Process Data", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtProcess.Focus();
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Process table / grid management
        // ─────────────────────────────────────────────────────────────────────

        private void InitializeProcessTable()
        {
            processTable = new DataTable();
            processTable.Columns.Add("Process ID", typeof(string));
            processTable.Columns.Add("Burst Time", typeof(int));
            processTable.Columns.Add("Priority", typeof(int));
            processTable.Columns.Add("Arrival Time", typeof(int));

            processDataGrid.DataSource = processTable;
            processDataGrid.AllowUserToAddRows = false;
            processDataGrid.AllowUserToDeleteRows = false;

            if (processDataGrid.Columns.Count > 0)
            {
                processDataGrid.Columns[0].Width = 100;
                processDataGrid.Columns[1].Width = 100;
                processDataGrid.Columns[2].Width = 100;
                processDataGrid.Columns[3].Width = 100;
                processDataGrid.VirtualMode = false;
                processDataGrid.RowHeadersVisible = false;
                processDataGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            }
        }

        private void SetProcessCount_Click(object sender, EventArgs e)
        {
            if (IsValidProcessCount(txtProcess.Text, out int processCount))
            {
                if (processCount > 50)
                {
                    var result = MessageBox.Show(
                        $"You are creating {processCount} processes. This may impact performance.\n\nContinue?",
                        "Large Dataset Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.No) { txtProcess.Focus(); return; }
                }

                processTable.Clear();
                for (int i = 0; i < processCount; i++)
                {
                    DataRow row = processTable.NewRow();
                    row["Process ID"] = $"P{i + 1}";
                    row["Burst Time"] = random.Next(1, 11);
                    row["Priority"] = i + 1;
                    row["Arrival Time"] = 0;
                    processTable.Rows.Add(row);
                }
                cmbLoadExample.SelectedIndex = 0;
            }
            else
            {
                MessageBox.Show($"Please enter a valid number of processes ({MIN_PROCESS_COUNT}-{MAX_PROCESS_COUNT})",
                    "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtProcess.Focus();
            }
        }

        private void GenerateRandom_Click(object sender, EventArgs e)
        {
            foreach (DataRow row in processTable.Rows)
            {
                row["Burst Time"] = random.Next(1, 21);
                row["Priority"] = random.Next(1, processTable.Rows.Count + 1);
                row["Arrival Time"] = random.Next(0, 10);
            }
        }

        private void ClearAll_Click(object sender, EventArgs e)
        {
            processTable.Clear();
            txtProcess.Text = DEFAULT_PROCESS_COUNT.ToString();
            cmbLoadExample.SelectedIndex = 0;
            txtProcess.Focus();
        }

        private void LoadExample_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbLoadExample.SelectedIndex <= 0 || processTable.Rows.Count == 0)
                return;

            switch (cmbLoadExample.SelectedIndex)
            {
                case 1:
                    foreach (DataRow row in processTable.Rows)
                    { row["Burst Time"] = random.Next(1, 6); row["Priority"] = random.Next(1, 5); row["Arrival Time"] = 0; }
                    break;
                case 2:
                    foreach (DataRow row in processTable.Rows)
                    { row["Burst Time"] = random.Next(1, 21); row["Priority"] = random.Next(1, 10); row["Arrival Time"] = random.Next(0, 5); }
                    break;
                case 3:
                    foreach (DataRow row in processTable.Rows)
                    { row["Burst Time"] = random.Next(10, 31); row["Priority"] = random.Next(1, 5); row["Arrival Time"] = random.Next(0, 10); }
                    break;
                case 4:
                    int priority = processTable.Rows.Count;
                    foreach (DataRow row in processTable.Rows)
                    { row["Burst Time"] = random.Next(5, 15); row["Priority"] = priority--; row["Arrival Time"] = 0; }
                    break;
            }
            cmbLoadExample.SelectedIndex = 0;
        }

        private void SaveData_Click(object sender, EventArgs e)
        {
            if (processTable.Rows.Count == 0)
            {
                MessageBox.Show("No process data to save.", "No Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
                saveDialog.DefaultExt = "csv";
                saveDialog.FileName = "ProcessData.csv";
                saveDialog.Title = "Save Process Data";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (var writer = new System.IO.StreamWriter(saveDialog.FileName))
                        {
                            writer.WriteLine("Process ID,Burst Time,Priority,Arrival Time");
                            foreach (DataRow row in processTable.Rows)
                                writer.WriteLine($"{row["Process ID"]},{row["Burst Time"]},{row["Priority"]},{row["Arrival Time"]}");
                        }
                        MessageBox.Show($"Saved to:\n{saveDialog.FileName}", "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error saving file: {ex.Message}", "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void LoadData_Click(object sender, EventArgs e)
        {
            using (var openDialog = new OpenFileDialog())
            {
                openDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
                openDialog.DefaultExt = "csv";
                openDialog.Title = "Load Process Data from CSV";

                if (openDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var loadedData = new List<ProcessData>();
                        using (var reader = new System.IO.StreamReader(openDialog.FileName))
                        {
                            var headerLine = reader.ReadLine();
                            if (headerLine == null)
                            {
                                MessageBox.Show("The CSV file is empty.", "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }

                            string line;
                            int lineNumber = 1;
                            while ((line = reader.ReadLine()) != null)
                            {
                                lineNumber++;
                                var parts = line.Split(',');
                                if (parts.Length != 4)
                                {
                                    MessageBox.Show($"Invalid format on line {lineNumber}.", "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    return;
                                }
                                try
                                {
                                    loadedData.Add(new ProcessData
                                    {
                                        ProcessID = parts[0].Trim(),
                                        BurstTime = int.Parse(parts[1].Trim()),
                                        Priority = int.Parse(parts[2].Trim()),
                                        ArrivalTime = int.Parse(parts[3].Trim())
                                    });
                                }
                                catch (FormatException)
                                {
                                    MessageBox.Show($"Invalid number on line {lineNumber}.", "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    return;
                                }
                            }
                        }

                        if (loadedData.Count == 0)
                        {
                            MessageBox.Show("No process data found.", "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        if (loadedData.Count > MAX_PROCESS_COUNT)
                            loadedData = loadedData.Take(MAX_PROCESS_COUNT).ToList();

                        processTable.Clear();
                        foreach (var process in loadedData)
                        {
                            DataRow row = processTable.NewRow();
                            row["Process ID"] = process.ProcessID;
                            row["Burst Time"] = process.BurstTime;
                            row["Priority"] = process.Priority;
                            row["Arrival Time"] = process.ArrivalTime;
                            processTable.Rows.Add(row);
                        }

                        txtProcess.Text = loadedData.Count.ToString();
                        cmbLoadExample.SelectedIndex = 0;
                        MessageBox.Show($"Loaded {loadedData.Count} processes.", "Load Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error loading file: {ex.Message}", "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Form load & default data
        // ─────────────────────────────────────────────────────────────────────

        private void ProcessTextBox_TextChanged(object sender, EventArgs e) { }

        private void RestartApp_Click(object sender, EventArgs e)
        {
            Hide();
            CpuSchedulerForm cpuScheduler = new CpuSchedulerForm();
            cpuScheduler.ShowDialog();
        }

        private void ApplyRoundedCorners(Button button, int radius = 15)
        {
            GraphicsPath path = new GraphicsPath();
            Rectangle rect = new Rectangle(0, 0, button.Width - 1, button.Height - 1);
            path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
            path.AddArc(rect.X + rect.Width - radius, rect.Y, radius, radius, 270, 90);
            path.AddArc(rect.X + rect.Width - radius, rect.Y + rect.Height - radius, radius, radius, 0, 90);
            path.AddArc(rect.X, rect.Y + rect.Height - radius, radius, radius, 90, 90);
            path.CloseAllFigures();
            button.Region = new Region(path);
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
        }

        private void CpuSchedulerForm_Load(object sender, EventArgs e)
        {
            sidePanel.Height = btnWelcome.Height;
            sidePanel.Top = btnWelcome.Top;
            listView1.View = View.Details;
            listView1.GridLines = true;

            listView1.Clear();
            listView1.Columns.Add("Information", 400, HorizontalAlignment.Left);
            listView1.Items.Add(new ListViewItem("No results yet – run a scheduling algorithm to see results here."));

            InitializeWelcomeContent();
            InitializeAboutContent();
            LoadDefaultProcessData();

            // Existing buttons
            ApplyRoundedCorners(btnSetProcessCount);
            ApplyRoundedCorners(btnGenerateRandom);
            ApplyRoundedCorners(btnClearAll);
            ApplyRoundedCorners(btnSaveData);
            ApplyRoundedCorners(btnLoadData);
            ApplyRoundedCorners(btnFCFS);
            ApplyRoundedCorners(btnSJF);
            ApplyRoundedCorners(btnPriority);
            ApplyRoundedCorners(btnRoundRobin);
            ApplyRoundedCorners(btnDarkModeToggle);
            // New algorithm buttons
            ApplyRoundedCorners(btnSRTF);
            ApplyRoundedCorners(btnHRRN);

            ApplyTheme();
            ShowPanel(welcomePanel);
        }

        private void LoadDefaultProcessData()
        {
            for (int i = 0; i < 5; i++)
            {
                DataRow row = processTable.NewRow();
                row["Process ID"] = $"P{i + 1}";
                row["Burst Time"] = new int[] { 6, 8, 7, 3, 4 }[i];
                row["Priority"] = i + 1;
                row["Arrival Time"] = new int[] { 0, 2, 4, 6, 8 }[i];
                processTable.Rows.Add(row);
            }
            txtProcess.Text = "5";
            cmbLoadExample.SelectedIndex = 0;
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Theme management
        // ─────────────────────────────────────────────────────────────────────

        private void ApplyTheme()
        {
            if (isDarkMode)
            { ApplyDarkTheme(); btnDarkModeToggle.Text = "☀️ Light Mode"; }
            else
            { ApplyLightTheme(); btnDarkModeToggle.Text = "🌙 Dark Mode"; }
        }

        private void ApplyDarkTheme()
        {
            this.BackColor = Color.FromArgb(45, 45, 48);
            panel1.BackColor = Color.FromArgb(37, 37, 38);
            sidePanel.BackColor = Color.FromArgb(0, 122, 204);

            ApplyDarkThemeToButton(btnWelcome);
            ApplyDarkThemeToButton(btnCpuScheduler);
            ApplyDarkThemeToButton(btnDashBoard);
            ApplyDarkThemeToButton(btnAbout);
            ApplyDarkThemeToButton(btnDarkModeToggle);

            restartApp.BackColor = Color.FromArgb(37, 37, 38);
            restartApp.ForeColor = Color.FromArgb(241, 241, 241);
            label1.ForeColor = Color.FromArgb(153, 153, 153);

            contentPanel.BackColor = Color.FromArgb(30, 30, 30);
            welcomePanel.BackColor = Color.FromArgb(30, 30, 30);
            schedulerPanel.BackColor = Color.FromArgb(30, 30, 30);
            resultsPanel.BackColor = Color.FromArgb(30, 30, 30);
            aboutPanel.BackColor = Color.FromArgb(30, 30, 30);

            welcomeTextBox.BackColor = Color.FromArgb(37, 37, 38);
            welcomeTextBox.ForeColor = Color.FromArgb(241, 241, 241);
            aboutTextBox.BackColor = Color.FromArgb(37, 37, 38);
            aboutTextBox.ForeColor = Color.FromArgb(241, 241, 241);

            labelProcess.ForeColor = Color.FromArgb(241, 241, 241);
            txtProcess.BackColor = Color.FromArgb(51, 51, 55);
            txtProcess.ForeColor = Color.FromArgb(241, 241, 241);

            processDataGrid.BackgroundColor = Color.FromArgb(37, 37, 38);
            processDataGrid.DefaultCellStyle.BackColor = Color.FromArgb(51, 51, 55);
            processDataGrid.DefaultCellStyle.ForeColor = Color.FromArgb(241, 241, 241);
            processDataGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(45, 45, 48);
            processDataGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(241, 241, 241);
            processDataGrid.GridColor = Color.FromArgb(62, 62, 66);

            cmbLoadExample.BackColor = Color.FromArgb(51, 51, 55);
            cmbLoadExample.ForeColor = Color.FromArgb(241, 241, 241);

            listView1.BackColor = Color.FromArgb(37, 37, 38);
            listView1.ForeColor = Color.FromArgb(241, 241, 241);

            ApplyDarkThemeToSchedulerButton(btnSetProcessCount);
            ApplyDarkThemeToSchedulerButton(btnGenerateRandom);
            ApplyDarkThemeToSchedulerButton(btnClearAll);
            ApplyDarkThemeToSchedulerButton(btnSaveData);
            ApplyDarkThemeToSchedulerButton(btnLoadData);
            ApplyDarkThemeToSchedulerButton(btnFCFS);
            ApplyDarkThemeToSchedulerButton(btnSJF);
            ApplyDarkThemeToSchedulerButton(btnPriority);
            ApplyDarkThemeToSchedulerButton(btnRoundRobin);
            ApplyDarkThemeToSchedulerButton(btnSRTF);
            ApplyDarkThemeToSchedulerButton(btnHRRN);
        }

        private void ApplyLightTheme()
        {
            this.BackColor = SystemColors.Control;
            panel1.BackColor = SystemColors.InactiveBorder;
            sidePanel.BackColor = Color.SeaGreen;

            ApplyLightThemeToButton(btnWelcome);
            ApplyLightThemeToButton(btnCpuScheduler);
            ApplyLightThemeToButton(btnDashBoard);
            ApplyLightThemeToButton(btnAbout);
            ApplyLightThemeToButton(btnDarkModeToggle);

            restartApp.BackColor = SystemColors.InactiveBorder;
            restartApp.ForeColor = Color.DarkBlue;
            label1.ForeColor = SystemColors.ControlText;

            contentPanel.BackColor = SystemColors.Control;
            welcomePanel.BackColor = SystemColors.Control;
            schedulerPanel.BackColor = SystemColors.Control;
            resultsPanel.BackColor = SystemColors.Control;
            aboutPanel.BackColor = SystemColors.Control;

            welcomeTextBox.BackColor = SystemColors.Window;
            welcomeTextBox.ForeColor = SystemColors.WindowText;
            aboutTextBox.BackColor = SystemColors.Window;
            aboutTextBox.ForeColor = SystemColors.WindowText;

            labelProcess.ForeColor = SystemColors.ControlText;
            txtProcess.BackColor = SystemColors.Window;
            txtProcess.ForeColor = SystemColors.WindowText;

            processDataGrid.BackgroundColor = SystemColors.Window;
            processDataGrid.DefaultCellStyle.BackColor = SystemColors.Window;
            processDataGrid.DefaultCellStyle.ForeColor = SystemColors.WindowText;
            processDataGrid.ColumnHeadersDefaultCellStyle.BackColor = SystemColors.Control;
            processDataGrid.ColumnHeadersDefaultCellStyle.ForeColor = SystemColors.ControlText;
            processDataGrid.GridColor = SystemColors.ControlDark;

            cmbLoadExample.BackColor = SystemColors.Window;
            cmbLoadExample.ForeColor = SystemColors.WindowText;

            listView1.BackColor = SystemColors.Window;
            listView1.ForeColor = SystemColors.WindowText;

            ApplyLightThemeToSchedulerButton(btnSetProcessCount);
            ApplyLightThemeToSchedulerButton(btnGenerateRandom);
            ApplyLightThemeToSchedulerButton(btnClearAll);
            ApplyLightThemeToSchedulerButton(btnSaveData);
            ApplyLightThemeToSchedulerButton(btnLoadData);

            btnFCFS.BackColor = Color.Beige;
            btnSJF.BackColor = Color.AntiqueWhite;
            btnPriority.BackColor = Color.Bisque;
            btnRoundRobin.BackColor = Color.PapayaWhip;
            btnSRTF.BackColor = Color.LightCyan;
            btnHRRN.BackColor = Color.LightYellow;

            foreach (var btn in new[] { btnFCFS, btnSJF, btnPriority, btnRoundRobin, btnSRTF, btnHRRN })
                btn.ForeColor = SystemColors.ControlText;
        }

        private void ApplyDarkThemeToButton(Button button)
        {
            button.BackColor = Color.FromArgb(37, 37, 38);
            button.ForeColor = Color.FromArgb(241, 241, 241);
            button.FlatAppearance.MouseOverBackColor = Color.FromArgb(62, 62, 66);
        }

        private void ApplyLightThemeToButton(Button button)
        {
            button.BackColor = SystemColors.InactiveBorder;
            button.ForeColor = SystemColors.ControlText;
            button.FlatAppearance.MouseOverBackColor = SystemColors.ButtonHighlight;
        }

        private void ApplyDarkThemeToSchedulerButton(Button button)
        {
            button.BackColor = Color.FromArgb(51, 51, 55);
            button.ForeColor = Color.FromArgb(241, 241, 241);
            button.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 122, 204);
        }

        private void ApplyLightThemeToSchedulerButton(Button button)
        {
            button.BackColor = SystemColors.ButtonFace;
            button.ForeColor = SystemColors.ControlText;
            button.FlatAppearance.MouseOverBackColor = Color.PaleGreen;
        }
    }

    /// <summary>Custom button class with rounded edges.</summary>
    public class RoundedButton : Button
    {
        private int borderRadius = 10;
        private Color borderColor = Color.FromArgb(200, 200, 200);

        public int BorderRadius { get { return borderRadius; } set { borderRadius = value; Invalidate(); } }
        public Color BorderColor { get { return borderColor; } set { borderColor = value; Invalidate(); } }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            Graphics g = pevent.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            GraphicsPath path = new GraphicsPath();
            Rectangle rect = new Rectangle(0, 0, Width - 1, Height - 1);
            path.AddArc(rect.X, rect.Y, borderRadius, borderRadius, 180, 90);
            path.AddArc(rect.X + rect.Width - borderRadius, rect.Y, borderRadius, borderRadius, 270, 90);
            path.AddArc(rect.X + rect.Width - borderRadius, rect.Y + rect.Height - borderRadius, borderRadius, borderRadius, 0, 90);
            path.AddArc(rect.X, rect.Y + rect.Height - borderRadius, borderRadius, borderRadius, 90, 90);
            path.CloseAllFigures();

            Region = new Region(path);

            using (SolidBrush brush = new SolidBrush(BackColor))
                g.FillPath(brush, path);

            using (Pen pen = new Pen(borderColor, 1))
                g.DrawPath(pen, path);

            TextRenderer.DrawText(g, Text, Font, ClientRectangle, ForeColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

            path.Dispose();
        }

        protected override void OnResize(EventArgs e) { base.OnResize(e); Invalidate(); }
    }
}