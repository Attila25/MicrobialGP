from lmfit import Minimizer, Parameters, report_fit
import numpy as np
import matplotlib.pylab as plt
from sklearn.preprocessing import MinMaxScaler

t = np.arange(0, 52, 1)
N = np.array([0.0014,
                0.0139,
                0.0135,
                0.0161,
                0.0193,
                0.0214,
                0.0254,
                0.0276,
                0.0343,
                0.0405,
                0.0503,
                0.0622,
                0.0799,
                0.1062,
                0.149,
                0.3556,
                0.4672,
                0.5504,
                0.6257,
                0.7284,
                0.8346,
                1.0024,
                1.1608,
                1.2714,
                1.3661,
                1.4373,
                1.4973,
                1.5394,
                1.5743,
                1.6001,
                1.631,
                1.6584,
                1.6765,
                1.6854,
                1.6931,
                1.6971,
                1.7022,
                1.7015,
                1.7009,
                1.6974,
                1.6972,
                1.6996,
                1.6949,
                1.6998,
                1.6975,
                1.6918,
                1.6934,
                1.6937,
                1.6929,
                1.6936,
                1.6826,
                1.6649])

start = 1
end = 100
width = end - start
#N = (N - N.min())/(N.max() - N.min()) * width + start
#t = np.arange(0, 24, 2)
#N = np.array([32500, 33000, 38000, 105000, 445000, 1430000, 3020000, 4720000, 5670000, 5870000, 5930000, 5940000])

N_rand = N

#Create object for parameter storing
params_gompertz = Parameters()
# add with tuples: (NAME VALUE VARY MIN  MAX  EXPR  BRUTE_STEP)
params_gompertz.add_many(('N_0', np.log(N_rand)[0] , True, 0, None, None, None),
                         ('N_max', np.log(N_rand)[-1], True, 0, None, None, None),
                         ('r_max', 0.62, True, None, None, None, None),
                         ('t_lag', 13, True, 0, None, None, None))#I see it in the graph

#Write down the objective function that we want to minimize, i.e., the residuals
def residuals_gompertz(params, t, data):
    '''Model a logistic growth and subtract data'''
    #Get an ordered dictionary of parameter values
    v = params.valuesdict()
    #Logistic model
    model = v['N_0'] + (v['N_max'] - v['N_0']) * \
    np.exp(-np.exp(v['r_max'] * np.exp(1) * (v['t_lag'] - t) / \
                   ((v['N_max'] - v['N_0']) * np.log(10)) + 1))
    #Return residuals
    return model - data

#Create a Minimizer object
minner = Minimizer(residuals_gompertz, params_gompertz, fcn_args=(t, np.log(N_rand)))
#Perform the minimization
fit_gompertz = minner.minimize()

#Sumarize results
report_fit(fit_gompertz)


plt.rcParams['figure.figsize'] = [10, 5]

#Gompertz
result_gompertz = np.log(N_rand) + fit_gompertz.residual
plt.plot(t, result_gompertz, 'g.', markersize = 15, label = 'Gompertz')
#Get a smooth curve by plugging a time vector to the fitted logistic model
t_vec = np.linspace(0, 52, 1000)
log_N_vec = np.ones(len(t_vec))
residual_smooth_gompertz = residuals_gompertz(fit_gompertz.params, t_vec, log_N_vec)
plt.plot(t_vec, residual_smooth_gompertz + log_N_vec, 'green', linestyle = '--', linewidth = 1)

#Plot data points
#plt.plot(t, np.log(N_rand), 'r+', markersize = 15,markeredgewidth = 2, label = 'Data')

#Plot legend
plt.legend(fontsize = 20)
plt.xlabel('t', fontsize = 20)
plt.ylabel(r'$\log(N_t)$', fontsize = 20)

plt.show()