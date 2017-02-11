import React from 'react'
import moment from 'moment'
import _ from 'lodash'
import { Tabs, Tab } from 'components/Tabs'
import Loading from 'components/Loading'
import Error from 'components/Error'

class View extends React.Component {
  static propTypes = {
    id: React.PropTypes.string.isRequired,
    getProfileRequest: React.PropTypes.func.isRequired,
    getProfile: React.PropTypes.object.isRequired
  }

  componentWillMount() {
    this.handleRefresh()
  }

  handleRefresh() {
    this.props.getProfileRequest({ id: this.props.id })
  }

  get list() {
    if (this.props.getProfile.loading) {
      return <Loading />
    }

    if (!this.props.getProfile.success) {
      return <Error />
    }

    return (
      <Tabs>
        <Tab title='Events'>
          <table className='table is-hover'>
            <thead>
              <tr>
                <th>Type</th>
                <th>Source</th>
                <th>Data</th>
                <th style={{ width: 200 }}>Created</th>
              </tr>
            </thead>
            <tbody>
              {_(this.props.getProfile.payload.events).sortBy('created').reverse().map((ent, index) => (
                <tr key={index}>
                  <td>{ent.type}</td>
                  <td>{ent.source}</td>
                  <td><pre>{JSON.stringify(ent.data, null, 2)}</pre></td>
                  <td>{moment(ent.created).fromNow()}</td>
                </tr>
              )).value()}
            </tbody>
          </table>
        </Tab>
        <Tab title='Bias'>
          <p>Work in progress...</p>
        </Tab>
        <Tab title='Attributes'>
          <table className='table is-hover'>
            <thead>
              <tr>
                <th>Attribute</th>
                <th>Value</th>
                <th style={{ width: 200 }}>Created</th>
              </tr>
            </thead>
            <tbody>
              {_(this.props.getProfile.payload.attributes).sortBy('created').reverse().map((ent, index) => (
                <tr key={index}>
                  <td>{ent.key}</td>
                  <td><pre>{ent.value}</pre></td>
                  <td>{moment(ent.lastActivity).fromNow()}</td>
                </tr>
              )).value()}
            </tbody>
          </table>
        </Tab>
      </Tabs>
    )
  }

  render() {
    return (
      <div>
        <h2 className='subtitle'>Profile - {this.props.id}</h2>
        {this.list}
      </div>
    )
  }
}

export default View
