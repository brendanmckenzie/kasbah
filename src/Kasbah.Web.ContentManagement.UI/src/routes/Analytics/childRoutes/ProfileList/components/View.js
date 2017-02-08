import React from 'react'
import _ from 'lodash'
import moment from 'moment'
import { Link } from 'react-router'
import Loading from 'components/Loading'
import Error from 'components/Error'

class View extends React.Component {
  static propTypes = {
    listProfilesRequest: React.PropTypes.func.isRequired,
    listProfiles: React.PropTypes.object.isRequired
  }

  componentWillMount() {
    this.props.listProfilesRequest()
  }

  get list() {
    if (this.props.listProfiles.loading) {
      return <Loading />
    }

    if (!this.props.listProfiles.success) {
      return <Error />
    }

    return (
      <table className='table is-hover'>
        <thead>
          <tr>
            <th>Id</th>
            <th style={{ width: 200 }}>Created</th>
          </tr>
        </thead>
        <tbody>
          {_(this.props.listProfiles.payload).sortBy('created').reverse().map(ent => (
            <tr key={ent.id}>
              <td className='is-link'><Link to={`/analytics/profile/${ent.id}`}>{ent.id}</Link></td>
              <td>{moment(ent.created).fromNow()}</td>
            </tr>
          )).value()}
        </tbody>
      </table>
    )
  }

  render() {
    return (
      <div>
        <h2 className='subtitle'>Profiles</h2>
        {this.list}
      </div>
    )
  }
}

export default View
