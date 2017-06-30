import React from 'react'
import PropTypes from 'prop-types'
import { connect } from 'react-redux'
import { actions as systemActions } from 'store/appReducers/system'
import Loading from 'components/Loading'

class View extends React.Component {
  static propTypes = {
    system: PropTypes.object.isRequired,
    listSites: PropTypes.func.isRequired
  }

  constructor() {
    super()

    this.state = { showModal: false }
  }

  componentWillMount() {
    if (!this.props.system.sites.loading) {
      this.handleRefresh()
    }
  }

  handleRefresh = () => {
    this.props.listSites()
  }

  get table() {
    if (this.props.system.loading) {
      return <Loading />
    }

    return (<table className='table is-striped'>
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
        {this.props.system.sites.list.map(ent => (
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

const mapStateToProps = (state) => ({
  system: state.system
})

const mapDispatchToProps = {
  ...systemActions
}

export default connect(mapStateToProps, mapDispatchToProps)(View)
