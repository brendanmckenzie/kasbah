import React from 'react'
import { Line } from 'react-chartjs-2'
import moment from 'moment'
import { Section, Container } from 'components/Layout'
import { makeApiRequest } from 'store/util'

class View extends React.Component {
  state = {
    session: null,
    sessionActivity: null,
    start: '2018-01-10',
    end: '2018-01-15',
    interval: 'hour'
  }

  componentDidMount() {
    makeApiRequest({
      method: 'GET',
      url: `/analytics/reporting/session?interval=${this.state.interval}&start=${this.state.start}&end=${this.state.end}`
    })
      .then(res => {
        this.setState({ session: res })
      })

    makeApiRequest({
      method: 'GET',
      url: `/analytics/reporting/session-activity?type=view&interval=${this.state.interval}&start=${this.state.start}&end=${this.state.end}`
    })
      .then(res => {
        this.setState({ sessionActivity: res })
      })
  }

  get chart() {
    if (!this.state.session) {
      return null
    }

    const start = moment(this.state.start)
    const end = moment(this.state.end)
    const diff = end.diff(start, this.state.interval)

    const labels = Array.from({ length: diff })
      .map((_, index) => moment(start).add(index, this.state.interval))
      .map(ent => ent.format('YYYY-MM-DD[T]HH:mm:ss'))

    const chartData = {
      labels: labels,
      datasets: [{
        label: 'Sessions',
        data: labels.map(dt => {
          const data = this.state.session.find(ent => ent.period === dt)
          return data ? data.count : null
        }),
        fill: false,
        borderColor: 'red'
      },
      {
        label: 'Session activity',
        data: labels.map(dt => {
          const data = this.state.sessionActivity.find(ent => ent.period === dt)
          return data ? data.count : null
        }),
        fill: false,
        borderColor: 'blue'
      }]
    }

    const chartOptions = {
      animated: false,
      animation: {
        duration: 0,
      },
      hover: {
        animationDuration: 0,
      },
      responsiveAnimationDuration: 0,
    }

    return <Line data={chartData} options={chartOptions} />
  }

  render() {
    return (
      <Section>
        <Container>
          {this.chart}
        </Container>
      </Section>
    )
  }
}

export default View
