import View from './components/View'
import { connect } from 'react-redux'
import { actions as mediaActions } from 'store/appReducers/media'

const mapStateToProps = (state) => ({
  media: state.media
})

const mapDispatchToProps = {
  ...mediaActions
}

export default connect(mapStateToProps, mapDispatchToProps)(View)
