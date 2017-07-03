import React from 'react'
import PropTypes from 'prop-types'
import moment from 'moment'
import { API_BASE } from 'store/util'
import DeleteButton from './DeleteButton'
import EditButton from './EditButton'
import { Column } from 'components/Layout'

class MediaItem extends React.Component {
  static propTypes = {
    item: PropTypes.object.isRequired
  }

  render() {
    const { item } = this.props

    return (
      <Column className='is-4'>
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
                <small className='level-item'>{moment.utc(item.created).fromNow()}</small>
              </div>
              <div className='level-right level-vertical'>
                <EditButton className='level-item button is-small is-secondary' media={item} />
                <DeleteButton className='level-item button is-small is-danger' media={item} />
              </div>
            </div>
          </div>
        </div>
        {this.modal}
      </Column>
    )
  }
}

export default MediaItem
