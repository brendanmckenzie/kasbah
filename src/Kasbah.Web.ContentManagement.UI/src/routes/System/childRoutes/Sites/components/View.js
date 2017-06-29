import React from 'react'
import PropTypes from 'prop-types'
import Loading from 'components/Loading'

class View extends React.Component {
  static propTypes = {
    listSitesRequest: PropTypes.func.isRequired,
    listSites: PropTypes.object.isRequired
  }

  constructor() {
    super()

    this.state = { showModal: false }
  }

  componentWillMount() {
    this.handleRefresh()
  }

  handleRefresh = () => {
    this.props.listSitesRequest()
  }

  get table() {
    if (this.props.listSites.loading) {
      return <Loading />
    }

    return (<table className='table is-hover'>
      <thead>
        <tr>
          <th>Alias</th>
          <th>Content root</th>
          <th>Default domain</th>
          <th>Domains</th>
          <th>Scheme</th>
        </tr>
      </thead>
      <tbody>
        {this.props.listSites.payload.map(ent => (
          <tr key={ent.alias}>
            <td>{ent.alias}</td>
            <td>{ent.contentRoot.join('/')}</td>
            <td>{ent.defaultDomain}</td>
            <td>{ent.domains.join(', ')}</td>
            <td>{ent.useSsl ? 'https' : 'http'}</td>
          </tr>
        ))}
      </tbody>
    </table>
    )
  }

  render() {
    return (
      <div>
        <div className='level'>
          <div className='level-left'>
            <h1 className='level-item title'>Sites</h1>
          </div>
        </div>
        {this.table}
      </div>
    )
  }
}

export default View
