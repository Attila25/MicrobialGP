import logging
import os
import re

from pathlib import Path
import pandas as pd
import openpyxl as opx
from collections import defaultdict
from OD import OD
from mCh import mCh
from yEGFP import yEGFP
from ChartProps import ChartProps
import json
import sys
import re
import time




def get_input_arguments():
    input_data_xlsx = sys.argv[1]
    input_layout_xlsx = sys.argv[2]
    output_folder = sys.argv[3]

    dt = Datahandler(input_data_xlsx, input_layout_xlsx, output_folder)
    dt.excel_data_to_csv()
    dt.fill_layout_table()
    dt.fill_data_classes()
    dt.to_json()
    dt.cleanup()


class Datahandler:
    def __init__(self, input_data_xlsx, input_layout_xlsx, output_folder):
        self.input_data_xlsx = input_data_xlsx
        self.input_layout_xlsx = input_layout_xlsx
        self.date_of_measure = str(re.search(r'\d+', self.input_data_xlsx).group(0))
        self.output_folder = Path(output_folder, self.date_of_measure)
        self.input_csv_name = Path(self.output_folder, self.input_data_xlsx.replace('xlsx', 'csv'))
        self.input_layout_name = Path(self.output_folder, self.input_data_xlsx.replace('.xlsx', '_Layout.csv'))
        self.input_key_name = Path(self.output_folder, self.input_data_xlsx.replace('.xlsx', '_Key.csv'))
        self.layout_key_pair_dict = defaultdict(list)
        self.OD_Class_list = []
        self.mCh_Class_list = []
        self.yEGFP_Class_list = []
        self.chart_props = None
        logging.basicConfig(filename="runtimelog",
                            filemode='a',
                            format='%(asctime)s,%(msecs)d %(name)s %(levelname)s %(message)s',
                            datefmt='%H:%M:%S',
                            level=logging.DEBUG)

        Path(self.output_folder).mkdir(parents=True, exist_ok=True)

    def excel_data_to_csv(self):
        logging.info("Converting excel data to csv")
        data_file = pd.read_excel(self.input_data_xlsx, index_col=None)
        data_file.to_csv(self.input_csv_name, encoding="utf-8", index_label=None, index=False, sep=";")

    def fill_layout_table(self):
        logging.info("Creating layout talbe")
        layout_file = pd.read_excel(self.input_layout_xlsx, sheet_name="Layout", index_col=None)
        key_file = pd.read_excel(self.input_layout_xlsx, sheet_name="Key", index_col=None)

        layout_file.to_csv(self.input_layout_name, encoding="utf-8", index_label=None, index=False, sep=";")
        key_file.to_csv(self.input_key_name, encoding="utf-8", index_label=None, index=False, sep=";")

        logging.info("Parsing the layout of the meauseremnt")
        layout_list = []
        with open(self.input_layout_name, "r") as layout_file:
            layout_file.readline()
            index = 1
            for line in layout_file:
                line = line.rstrip()
                id = line.split(";")[0]
                for i in range(1, len(line.split(";"))):
                    layout_list.append(id + str(index) + ";" + line.split(";")[i])
                    index = index + 1
                index = 1
            layout_file.close()

        logging.info("Parsing the keys of the meauseremnt")
        key_list = []
        with open(self.input_key_name, "r") as key_file:
            for line in key_file:
                line = line.rstrip()
                key_list.append(line)
            key_file.close()

        logging.info("Creating the dictionary of the tribes")
        for key_items in key_list:
            id = key_items.split(";")[0]
            name = key_items.split(";")[1]

            for layout_items in layout_list:
                if layout_items.split(";")[1] == id:
                    self.layout_key_pair_dict[name].append(layout_items.split(";")[0])

    def fill_data_classes(self):

        logging.info("Creating the Data tables")
        list_for_OD = []
        list_for_mCh = []
        list_for_yEGFP = []
        check_class = ""

        with open(self.input_csv_name, "r") as datafile:
            logging.info("Reading the Data csv")
            for line in datafile:
                if check_class == "OD" and not line.startswith(";") and line.split(";")[0] != "mCh":
                    list_for_OD.append(line.strip())
                elif check_class == "mCh" and not line.startswith(";") and line.split(";")[0] != "yEGFP":
                    list_for_mCh.append(line.strip())
                elif check_class == "yEGFP" and not line.startswith(";") and "End" not in line.split(";")[0]:
                    list_for_yEGFP.append(line.strip())

                if "OD" in line.split(";")[0]:
                    check_class = "OD"
                elif "mCh" in line.split(";")[0]:
                    check_class = "mCh"
                elif "yEGFP" in line.split(";")[0]:
                    check_class = "yEGFP"
                elif "End" in line.split(";")[0]:
                    check_class = "Done"

        logging.info("Reforming the data to the valid structure")
        first_line = list_for_OD[0]

        list_for_OD.pop(0)
        list_for_mCh.pop(0)
        list_for_yEGFP.pop(0)

        logging.info("Creating and filling the classes")
        for k, v in self.layout_key_pair_dict.items():
            OD_data = defaultdict(list)
            mCh_data = defaultdict(list)
            yEGFP_data = defaultdict(list)

            logging.info("Reading the data values")
            for data in v:
                col = first_line.split(";").index(data)
                for items in list_for_OD:
                    OD_data[data].append(float(items.split(";")[col]))
                for items in list_for_mCh:
                    mCh_data[data].append(float(items.split(";")[col]))
                for items in list_for_yEGFP:
                    yEGFP_data[data].append(float(items.split(";")[col]))

            logging.info("Calculating the averages")
            average_OD = [round(sum(vals) / len(vals), 4) for vals in zip(*OD_data.values())]
            max_values_OD = [round(max(vals)) for vals in zip(*OD_data.values())]
            min_values_OD = [round(min(vals)) for vals in zip(*OD_data.values())]

            average_mCh = [round(sum(vals) / len(vals), 4) for vals in zip(*mCh_data.values())]
            max_values_mCh = [round(max(vals)) for vals in zip(*mCh_data.values())]
            min_values_mCh = [round(min(vals)) for vals in zip(*mCh_data.values())]

            average_yEGFP = [round(sum(vals) / len(vals), 4) for vals in zip(*yEGFP_data.values())]
            max_values_yEGFP = [round(max(vals)) for vals in zip(*yEGFP_data.values())]
            min_values_yEGFP = [round(min(vals)) for vals in zip(*yEGFP_data.values())]

            logging.info("Adding the new values to the class")
            self.OD_Class_list.append(OD(k, 0, OD_data, average_OD, max_values_OD, min_values_OD))
            self.mCh_Class_list.append(mCh(k, 0, mCh_data, average_mCh, max_values_mCh, min_values_mCh))
            self.yEGFP_Class_list.append(yEGFP(k, 0, yEGFP_data, average_yEGFP, max_values_yEGFP, min_values_yEGFP))

            logging.info("Processing of " + k + " finished");

        logging.info("Substracting the background values")
        Background_values_OD = [x.average for x in self.OD_Class_list if x.name == "BackGround"]
        Background_values_mCh = [x.average for x in self.mCh_Class_list if x.name == "BackGround"]
        Background_values_yEGFP = [x.average for x in self.yEGFP_Class_list if x.name == "BackGround"]

        logging.info("Updating the classes with the new values")
        for datas in self.OD_Class_list:
            for k, v in datas.data.items():
                new_values = [abs(round(a - b, 4)) for a, b in zip(v, *Background_values_OD)]
                datas.data[k] = new_values

        for datas in self.mCh_Class_list:
            for k, v in datas.data.items():
                new_values = [abs(round(a - b, 4)) for a, b in zip(v, *Background_values_mCh)]
                datas.data[k] = new_values

        for datas in self.yEGFP_Class_list:
            for k, v in datas.data.items():
                new_values = [abs(round(a - b, 4)) for a, b in zip(v, *Background_values_yEGFP)]
                datas.data[k] = new_values

        Cycle_number = len(*Background_values_OD)
        self.chart_props = ChartProps(self.date_of_measure, Cycle_number)

        logging.info("Processing of the data finished")

    def obj_dict(self, obj):
        return obj.__dict__

    def to_json(self):
        logging.info("Creating JSON format")
        with open(Path.joinpath(self.output_folder, 'OD_' + self.date_of_measure + '.json'), 'w', encoding='utf-8') as f:
            json.dump(self.OD_Class_list, f, ensure_ascii=False, indent=4, default=self.obj_dict)

        with open(Path.joinpath(self.output_folder, 'mCh_' + self.date_of_measure + '.json'), 'w', encoding='utf-8') as f:
            json.dump(self.mCh_Class_list, f, ensure_ascii=False, indent=4, default=self.obj_dict)

        with open(Path.joinpath(self.output_folder, 'yEGFP_' + self.date_of_measure + '.json'), 'w', encoding='utf-8') as f:
            json.dump(self.yEGFP_Class_list, f, ensure_ascii=False, indent=4, default=self.obj_dict)

        with open(Path.joinpath(self.output_folder, 'ChartProps_' + self.date_of_measure + '.json'), 'w', encoding='utf-8') as f:
            json.dump(self.chart_props, f, ensure_ascii=False, indent=4, default=self.obj_dict)

    def cleanup(self):
        logging.info("Deleting unnecessary  files")
        if Path.is_file(self.input_csv_name):
            Path.unlink(self.input_csv_name)

        if Path.is_file(self.input_key_name):
            Path.unlink(self.input_key_name)

        if Path.is_file(self.input_layout_name):
            Path.unlink(self.input_layout_name)

        print(str(self.output_folder))

get_input_arguments()