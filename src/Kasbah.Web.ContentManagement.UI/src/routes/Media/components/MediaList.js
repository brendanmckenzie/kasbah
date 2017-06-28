import React from 'react'
import PropTypes from 'prop-types'
import Loading from 'components/Loading'
import MediaItem from './MediaItem'

const MediaList = ({ media }) => {
  if (media.loading) {
    return <Loading />
  }

  return (
    <div className='columns is-multiline'>
      {media.list.items.map((ent, index) => (
        <MediaItem key={ent.id} item={ent} />
      ))}
    </div>
  )
}

MediaList.propTypes = {
  media: PropTypes.object.isRequired
}

export default MediaList
