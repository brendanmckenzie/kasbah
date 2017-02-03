import { connect } from 'react-redux'
import { action as getDetailRequest } from '../modules/getDetail'
import { action as putDetailRequest } from '../modules/putDetail'

import View from '../components/View'

const mapDispatchToProps = {
  getDetailRequest,
  putDetailRequest
}

const mapStateToProps = (state, ownProps) => ({
  id: ownProps.params.id,
  getDetail: state.getDetail,
  putDetail: state.putDetail
})

export default connect(mapStateToProps, mapDispatchToProps)(View)
