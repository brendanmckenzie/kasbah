import React from 'react'
import PropTypes from 'prop-types'
import { connect } from 'react-redux'
import { actions as securityActions } from 'store/appReducers/security'
import Loading from 'components/Loading'
import UserListRow from './UserListRow'

class View extends React.Component {
  static propTypes = {
    listUsers: PropTypes.func.isRequired,
    security: PropTypes.object.isRequired
  }

  componentWillMount() {
    if (!this.props.security.users.loaded) {
      this.handleRefresh()
    }
  }

  handleRefresh = () => {
    this.props.listUsers()
  }

  handleSave = (values) => {
    if (this.state.editing) {
      this.props.putUserRequest(values)
    } else {
      this.props.createUserRequest(values)
    }
  }

  // get modal() {
  //   if (!this.state.showModal) { return null }

  //   return (<UserForm
  //     onClose={this.handleHideModal}
  //     onSubmit={this.handleSave}
  //     initialValues={this.state.editing}
  //     loading={this.props.createUser.loading || this.props.putUser.loading} />)
  // }

  get table() {
    if (this.props.security.users.loading) {
      return <Loading />
    }

    return (<table className='table is-hover'>
      <thead>
        <tr>
          <th>Name</th>
          <th style={{ width: 200 }}>Username</th>
          <th style={{ width: 200 }}>Email</th>
          <th style={{ width: 200 }}>Last login</th>
          <th />
        </tr>
      </thead>
      <tbody>
        {this.props.security.users.list.map(ent => <UserListRow key={ent.id} user={ent} />)}
      </tbody>
    </table>
    )
  }

  render() {
    return (
      <div>
        <div className='level'>
          <div className='level-left'>
            <h1 className='level-item subtitle'>Users</h1>
          </div>
          <div className='level-right'>
            <button
              type='button'
              className='level-item button is-primary'
              onClick={this.handleShowModal}>New user</button>
          </div>
        </div>
        {this.table}
      </div>
    )
  }
}

const mapStateToProps = (state, ownProps) => ({
  security: state.security
})

const mapDispatchToProps = {
  ...securityActions
}

export default connect(mapStateToProps, mapDispatchToProps)(View)
