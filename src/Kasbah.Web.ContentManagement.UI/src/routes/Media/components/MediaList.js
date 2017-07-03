import React from 'react'
import PropTypes from 'prop-types'
import Loading from 'components/Loading'
import MediaItem from './MediaItem'
import { Columns } from 'components/Layout'

const MediaList = ({ media }) => {
  if (media.loading) {
    return <Loading />
  }

  return (
    <Columns className='is-multiline'>
      {media.list.items.map((ent, index) => (
        <MediaItem key={ent.id} item={ent} />
      ))}
    </Columns>
  )
}

MediaList.propTypes = {
  media: PropTypes.object.isRequired
}

export default MediaList
