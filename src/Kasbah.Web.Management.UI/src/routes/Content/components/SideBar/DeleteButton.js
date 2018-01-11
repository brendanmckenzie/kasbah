import React from 'react'
import { connect } from 'react-redux'
import PropTypes from 'prop-types'
import { actions as uiActions } from 'store/appReducers/ui'
import { actions as contentActions } from 'store/appReducers/content'

class DeleteButton extends React.Component {
  static propTypes = {
    node: PropTypes.object.isRequired,
    showModal: PropTypes.func.isRequired,
    hideModal: PropTypes.func.isRequired,
    deleteNode: PropTypes.func.isRequired
  }

  handleClick = () => {
    const doDelete = () => {
      this.props.deleteNode(this.props.node)
    }

    const title = 'Delete node'
    const content = <div className='content'>
      <p>Are you sure you wish to delete this node?</p>
      <p>It is not possible to recover from this action.</p>
    </div>

    const buttons = [
      <button type='button' className='button' onClick={this.props.hideModal}>Cancel</button>,
      <button type='button' className='button is-danger' onClick={doDelete}>Delete</button>
    ]

    this.props.showModal(title, content, buttons)
  }

  render() {
    return (
      <button className='button is-danger' type='button' onClick={this.handleClick}>
        <span className='icon is-small'><i className='fa fa-trash' /></span>
        <span>Delete</span>
      </button>
    )
  }
}

const mapDispatchToProps = {
  ...uiActions,
  ...contentActions
}

export default connect(null, mapDispatchToProps)(DeleteButton)
