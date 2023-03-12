import sys

import numpy as np
import matplotlib.pyplot as plt

from keras.models import Sequential
from keras.layers import Dense, Dropout, LSTM
from keras.optimizers import Adam
from sklearn.preprocessing import MinMaxScaler


def data_creation():
    inputarray = []
    with open("input.txt") as f:
        for lines in f:
            inputarray.append(float(lines.strip().replace(",", "")))

    data = np.array(inputarray).reshape(-1, 1)

    training_size = round(len(data) * 0.80)

    data_train = data[:training_size]
    data_test = data[training_size:]

    print("Tréning adatok: ", data_train.shape)
    print("Teszt adatok: ", data_test.shape)

    plt.plot(inputarray, linewidth=1, color='blue', label='Growth')
    plt.gcf().autofmt_xdate()
    plt.show()

    training(data_train, data_test)


def training(data_train, data_test):
    window_size = 52
    Training_period = []
    Training_growth = []

    Testing_period = []
    Testing_growth = []

    sc = MinMaxScaler(feature_range=(0, 1))

    data_train = sc.fit_transform(data_train)
    data_test = sc.fit_transform(data_test)

    for i in range(window_size, len(data_train)):
        Training_period.append(data_train[i - window_size:i, 0])
        Training_growth.append(data_train[i, 0])

    for i in range(window_size, len(data_test)):
        Testing_period.append(data_test[i - window_size:i, 0])
        Testing_growth.append(data_test[i, 0])

    Training_period, Training_growth = np.array(Training_period), np.array(Training_growth)
    Testing_period, Testing_growth = np.array(Testing_period), np.array(Testing_growth)

    print("Training_period shape: ", Training_period.shape)
    print("Training_growth shape: ", Training_growth.shape)
    print("Testing_period shape: ", Testing_period.shape)
    print("Testing_period shape: ", Testing_growth.shape)

    print(Training_period[:2])
    print(Training_growth)

    Training_period = np.reshape(Training_period, (Training_period.shape[0], Training_period.shape[1], 1))
    Testing_period = np.reshape(Testing_period, (Testing_period.shape[0], Testing_period.shape[1], 1))

    model = Sequential()

    model.add(LSTM(units=180, return_sequences=True, input_shape=(Training_period.shape[1], 1)))
    model.add(Dropout(0.3))

    model.add(LSTM(units=180))
    model.add(Dropout(0.2))

    model.add(Dense(units=1))

    model.compile(optimizer=Adam(learning_rate=0.001, name="Adam"), loss='mean_squared_error')

    model.summary()

    history = model.fit(Training_period, Training_growth, epochs=100, batch_size=48, verbose=1)

    expected_growth = model.predict(Testing_period)
    expected_growth = sc.inverse_transform(expected_growth)
    Testing_growth = sc.inverse_transform(Testing_growth.reshape(-1, 1))
    print(expected_growth[:10])

    plt.figure(figsize=(12, 6))
    plt.plot(Testing_growth, color='green', label='Real Growth')
    plt.plot(expected_growth, color='blue', label='Predicted Growth')
    plt.legend()
    plt.show()

    for i in range(0, len(expected_growth)):
        print(expected_growth[i])
    with open("output.txt", mode='w') as w:
        for i in range(0, len(expected_growth)):
            w.write(str(expected_growth[i][0]) + '\n')
        w.flush()
        w.close()


data_creation()
