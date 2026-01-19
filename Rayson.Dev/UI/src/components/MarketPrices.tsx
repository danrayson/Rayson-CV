import React, { useEffect, useRef } from 'react';
import Highcharts from 'highcharts/highstock';
import HighchartsReact from 'highcharts-react-official';

const MarketPrices: React.FC = () => {
  const chartRef = useRef<HighchartsReact.RefObject>(null);

  useEffect(() => {
    const interval = setInterval(() => {
      if (chartRef.current) {
        const chart = chartRef.current.chart;
        const series = chart.series[0];
        const time = (new Date()).getTime();
        const lastPoint = series.data[series.data.length - 1];

        // Generate new OHLC data
        const open = lastPoint ? (lastPoint as any).close : 100;
        const close = open + Math.random() * 2 - 1;
        const high = Math.max(open, close) + Math.random();
        const low = Math.min(open, close) - Math.random();

        series.addPoint([time, open, high, low, close], true, false);
      }
    }, 30);

    return () => clearInterval(interval);
  }, []);

  const options: Highcharts.Options = {
    chart: {
      // styledMode: true
    },
    title: {
      text: 'Market Prices (OHLC)'
    },
    series: [{
      type: 'candlestick',
      name: 'Stock Price',
      data: [
        [Date.now(), 100, 102, 98, 101]
      ],
      color: '#FF7F7F',
      upColor: '#90EE90',
      lastPrice: {
        enabled: true,
        label: {
          enabled: true,
          backgroundColor: '#FF7F7F'
        }
      }
    }],
    xAxis: {
      overscroll: 500000,
      range: 4 * 200000,
      gridLineWidth: 1
    },
    navigator: {
      series: {
        color: '#000000'
      }
    },
    rangeSelector: {
      buttons: [{
        type: 'second',
        count: 1,
        text: '1s'
      },{
        type: 'minute',
        count: 15,
        text: '15m'
      }, {
        type: 'hour',
        count: 1,
        text: '1h'
      }, {
        type: 'all',
        count: 1,
        text: 'All'
      }],
      selected: 1,
      inputEnabled: false
    }
  };

  return (
    <div className="full-screen">
      <h1 className='topBit'>Market Prices</h1>
      <HighchartsReact
        highcharts={Highcharts}
        options={options}
        constructorType={'stockChart'}
        ref={chartRef}
        containerProps={{ className: 'fillRemainder' }}
      />
    </div>
  );
};

export default MarketPrices;
