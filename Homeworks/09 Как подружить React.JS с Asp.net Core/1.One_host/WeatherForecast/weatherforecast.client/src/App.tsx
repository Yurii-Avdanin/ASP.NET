import 'bootstrap/dist/css/bootstrap.min.css';
import { useEffect, useState } from 'react';
import Headline from './components/Panels/Headline';
import axios from 'axios';

interface Forecast {
    date: string;
    temperatureC: number;
    temperatureF: number;
    summary: string;
}

const url_WeatherForecast = `/WeatherForecast`;

function App() {
    const [forecasts, setForecasts] = useState<Forecast[]>([]);

    useEffect(() => {
        populateWeatherData();
    }, []);

    const contents = forecasts === undefined
        ? <p><em>Loading... Please refresh once the ASP.NET backend has started.</em></p>
        : <table className="table table-striped" aria-labelledby="tableLabel">
            <thead>
                <tr>
                    <th>Date</th>
                    <th>Temp. (C)</th>
                    <th>Temp. (F)</th>
                    <th>Summary</th>
                </tr>
            </thead>
            <tbody>
                {forecasts.map(forecast =>
                    <tr key={forecast.date}>
                        <td>{forecast.date}</td>
                        <td>{forecast.temperatureC}</td>
                        <td>{forecast.temperatureF}</td>
                        <td>{forecast.summary}</td>
                    </tr>
                )}
            </tbody>
        </table>;

    return (
        <div>
            <Headline text={'Домашняя работа №9'}/>

            <h1 id="tableLabel">Прогноз погоды</h1>
            <p>This component demonstrates fetching data from the server.</p>
            {contents}
        </div>
    );

    async function populateWeatherData() {
        try {
            const response = await axios.get(url_WeatherForecast);
            console.log('Статус ответа:', response.status);            

            //console.log('Тип response.data:', typeof response.data);
            //console.log('Содержимое response.data:', response.data);

            if (Array.isArray(response.data)) {
                setForecasts(response.data);
            }
            else {
                throw new Error('Ошибка формата данных. Возможно, бэкенд не работает.');
            }
        }
        catch (error) {
            if (axios.isAxiosError(error)) {            
                console.log('Статус ошибки:', error.response?.status);
                console.log('Данные ошибки:', error.response?.data);
            } else {
                console.log('Неизвестная ошибка:', error);
            }         
        }
    }
}

export default App;