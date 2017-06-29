import React from 'react'
import { connect } from 'react-redux'
import PropTypes from 'prop-types'
import { actions as uiActions } from 'store/appReducers/ui'
import { actions as mediaActions } from 'store/appReducers/media'

class DeleteButton extends React.Component {
  static propTypes = {
    media: PropTypes.object.isRequired,
    className: PropTypes.string,
    showModal: PropTypes.func.isRequired,
    hideModal: PropTypes.func.isRequired,
    deleteMedia: PropTypes.func.isRequired
  }

  handleClick = () => {
    const doDelete = () => {
      this.props.deleteMedia(this.props.media)
    }

    const title = 'Delete media'
    const content = <div className='content'>
      <p>Are you sure you wish to delete this media from your library?</p>
    </div>

    const buttons = [
      <button type='button' className='button' onClick={this.props.hideModal}>Cancel</button>,
      <button type='button' className='button is-danger' onClick={doDelete}>Delete</button>
    ]

    this.props.showModal(title, content, buttons)
  }

  render() {
    return (
      <button className={this.props.className} type='button' onClick={this.handleClick}>
        <span className='icon is-small'><i className='fa fa-trash' /></span>
        <span>Delete</span>
      </button>
    )
  }
}

const mapDispatchToProps = {
  ...uiActions,
  ...mediaActions
}

export default connect(null, mapDispatchToProps)(DeleteButton)
