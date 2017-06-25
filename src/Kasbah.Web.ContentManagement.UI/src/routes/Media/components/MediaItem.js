import React from 'react'
import PropTypes from 'prop-types'
import moment from 'moment'
import ItemButton from 'components/ItemButton'
import { API_BASE } from 'store/util'
import MediaForm from 'forms/MediaForm'

class MediaItem extends React.Component {
  static propTypes = {
    item: PropTypes.object.isRequired,
    onDelete: PropTypes.func.isRequired,
    onEdit: PropTypes.func.isRequired,
    putMedia: PropTypes.object.isRequired
  }

  constructor() {
    super()

    this.state = {
      showModal: false
    }
  }

  componentWillReceiveProps(nextProps) {
    if (nextProps.putMedia.success && nextProps.putMedia.loading !== this.props.putMedia.loading) {
      this.handleToggleModal(false)
    }
  }

  handleToggleModal = (showModal) => () => {
    this.setState({ showModal })
  }

  get modal() {
    if (!this.state.showModal) {
      return null
    }

    return <MediaForm
      onSubmit={this.props.onEdit}
      initialValues={this.props.item}
      onClose={this.handleToggleModal(false)}
      loading={this.props.putMedia.loading} />
  }

  render() {
    const { item, onDelete } = this.props

    return (
      <div className='column is-4'>
        <div className='media'>
          <figure className='media-left'>
            <span className='image is-64x64'>
              <img src={`${API_BASE}/media?id=${item.id}&width=128&height=128`} />
            </span>
          </figure>
          <div className='media-content'>
            <div className='level'>
              <div className='level-left level-vertical'>
                <strong className='level-item'>{item.fileName}</strong>
                <span className='level-item'>{item.contentType}</span>
                <small className='level-item'>{moment(item.created).fromNow()}</small>
              </div>
              <div className='level-right level-vertical'>
                <ItemButton
                  className='level-item button is-small is-secondary'
                  onClick={this.handleToggleModal(true)}
                  item={item}>
                  <span className='icon is-small'>
                    <i className='fa fa-pencil' />
                  </span>
                  <span>Edit</span>
                </ItemButton>
                <ItemButton className='level-item button is-small is-danger' onClick={onDelete} item={item}>
                  <span className='icon is-small'>
                    <i className='fa fa-trash' />
                  </span>
                  <span>Delete</span>
                </ItemButton>
              </div>
            </div>
          </div>
        </div>
        {this.modal}
      </div>
    )
  }
}

export default MediaItem
