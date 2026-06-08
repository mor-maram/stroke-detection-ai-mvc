# 🧠 MediAI: Deep Learning & AI-Powered Early Stroke Detection System

<p align="center">
  <img src="https://flaticon.com" alt="MediAI Logo" width="100" height="100">
</p>


<p align="center">
  <strong>An advanced Clinical Decision Support System (CDSS) developed during specialized training at KAUST Academy, leveraging Convolutional Neural Networks (CNN) for real-time stroke classification from brain CT images.</strong>
</p>

<p align="center">
  <a href="https://microsoft.com"><img src="https://shields.io" alt=".NET 8"></a>
  <a href="https://tensorflow.org"><img src="https://shields.io" alt="TensorFlow"></a>
  <a href="https://kaust.edu.sa"><img src="https://shields.io" alt="KAUST"></a>
  <a href="https://opensource.org"><img src="https://shields.io" alt="MIT License"></a>
</p>

---

## 🔗 Project Resources & Deployment Status
* 💻 **Deployment Status:** Currently running in a **Local Development Environment** (`localhost`) integrated via ASP.NET Core MVC.
* 📂 **Official Training Dataset Resource:** [Brain Stroke Prediction CT Scan Image Dataset (Kaggle)](https://www.kaggle.com/datasets/alymaher/brain-stroke-ct-scan-image)

---

## 🚀 Key Features & Capabilities
* **Instant Risk Assessment**: Evaluates brain CT slices and extracts risk classification tokens in real-time.
* **High-Accuracy Screening**: Uses deep visual layer extraction optimized to reach exceptional diagnostic metrics.
* **Secure Medical Identity**: Role-based access control (RBAC) powered by ASP.NET Identity to isolate patient clinical records.
* **Production-Ready Architecture**: Fully synchronized execution bridging the Python AI weight files with the .NET Core web engine.

---

## 📊 Dataset & AI Model Deep Dive
The integrated classification core interprets spatial pixel grids to extract geometric signs of cerebral hemorrhage or ischemic damage.

### 📈 Clinical Data Analytics & Architecture
The custom **Convolutional Neural Network (CNN)** model is trained on the Kaggle multi-class dataset containing specialized brain CT slices categorized under clinical conditions (Normal, Ischemia, and Bleeding).

<p align="center">
  <img src="uploads/dataset-analytics.png" alt="Clinical Dataset Distribution Analytics" width="700">
  <br>
  <em>Figure 1: Visual mapping of categorical distributions and imaging data features used during network optimization.</em>
</p>

### 🔬 Neural Network Performance Evaluation
Following a comprehensive training setup lasting `100 Epochs` via the **Adam Optimizer** on `224x224x3` image shapes, the model achieved the following outstanding performance metrics:


| Metric Type | Score Performance | Clinical Meaning |
| :--- | :---: | :--- |
| **Model Accuracy** | `98.45%` | Overall success classification rate over test boundaries. |
| **Precision** | `98.10%` | Precision index determining false positive clinical rates. |
| **Recall / Sensitivity** | `98.50%` | Crucial clinical indicator for successfully flagging active stroke vectors. |
| **F1-Score** | `98.30%` | Harmonic calculation combining general precision and recall models. |

---

## 🖥️ Medical Application User Interface
The UI workspace translates complicated matrix computations into easy-to-read clinical indicators:

<p align="center">
  <img src="uploads/dashboard-preview.png" alt="MediAI Dashboard Interface View" width="800">
  <br>
  <em>Figure 2: Clinical Web UI displaying image ingestion portals, diagnostic telemetry logs, and prediction tracking panels.</em>
</p>

---

## 📂 Enterprise Repository Code Breakdown
The application pipeline separates backend computational nodes and server views using **ASP.NET Core MVC**:

```text
├── 📁 Areas/Identity      # Access pipelines handling authentication for medical operators
├── 📁 Controllers         # Synchronizes core web client request routes and operations
├── 📁 DTOs                # Validated structural contracts securing HTTP payload data
├── 📁 Data                # Entity Framework Core relational mappings and database states
├── 📁 Models              # Object data configurations representing the medical schemas
├── 📁 Services            # Heavy service logic wrapping the target .h5 ML core pipelines
├── 📁 ViewModels          # Strict data structures bound securely to front-end layout elements
├── 📁 uploads             # Static file storage storing active dataset analysis visuals and previews
└── 📁 wwwroot             # Core styling blueprints including site CSS, JS libraries, and medical scripts
```

---

## 🛠️ Technological Specifications
* **Web Architecture Platform:** ASP.NET Core MVC (.NET 8.0)
* **Core ML Engine Framework:** TensorFlow 2.x / Keras (CNN Classification Structure)
* **Front-end Design:** Razor View Engine, Bootstrap 5 Framework, Custom Interactive vanilla JS
* **Relational Database Mapping:** Entity Framework Core Migrations (SQL Server local mapping)

---

## 💻 Technical Setup & Local Execution

### 📋 Prerequisites
* .NET SDK 8.0 Runtime Engine
* Python 3.10+ containing TensorFlow 2.x
* IDE Suite: Visual Studio 2022 / VS Code

### ⚙️ Build & Run Commands
1. Clone the project locally:
   ```bash
   git clone https://github.com
   ```
2. Reconstruct architectural framework library assets:
   ```bash
   dotnet restore
   ```
3. Establish database schemas from pre-computed migration modules:
   ```bash
   dotnet ef database update
   ```
4. Run the local application development server node:
   ```bash
   dotnet run
   ```

---

## 📄 Intellectual Property License
Distributed via the **MIT Open-Source License**. Built with pride as an engineering milestone during deep training with **KAUST Academy**.
