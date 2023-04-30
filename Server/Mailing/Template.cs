namespace DocsWASM.Server.Mailing
{
	public static class Template
	{
		private static string layout(string html)
		{
			return @"<html>
	<head>
		<title></title>
		<meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"">
			<meta name=""viewport"" content=""width=device-width,initial-scale=1"">
				<style>
*{box-sizing:border-box}body{margin:0;padding:0}a[x-apple-data-detectors]{color:inherit!important;text-decoration:inherit!important}#MessageViewBody a{color:inherit;text-decoration:none}p{line-height:inherit}.desktop_hide,.desktop_hide table{mso-hide:all;display:none;max-height:0;overflow:hidden}.image_block img+div{display:none} @media (max-width:720px){.row-content{width:100%!important}.mobile_hide{display:none}.stack .column{width:100%;display:block}.mobile_hide{min-height:0;max-height:0;max-width:0;overflow:hidden;font-size:0}.desktop_hide,.desktop_hide table{display:table!important;max-height:none!important}}
</style>
			</head>
			<body style=""background-color:#fff;margin:0;padding:0;-webkit-text-size-adjust:none;text-size-adjust:none"">
				<table class=""nl-container"" width=""100%"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace:0;mso-table-rspace:0;background-color:#fff"">
					<tbody>
							"+ 
							html
							+
@"					</tbody>
				</table>
			</body>
		</html>";
		}
		public static string VerificationCode(string code)
		{
			var html = @"
						<tr>
							<td>
								<table class=""row row-1"" align=""center"" width=""100%"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace:0;mso-table-rspace:0"">
									<tbody>
										<tr>
											<td>
												<table class=""row-content stack"" align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace:0;mso-table-rspace:0;background-color:#efefef;color:#000;width:700px"" width=""700"">
													<tbody>
														<tr>
															<td class=""column column-1"" width=""100%"" style=""mso-table-lspace:0;mso-table-rspace:0;font-weight:400;text-align:left;padding-bottom:20px;padding-top:30px;vertical-align:top;border-top:0;border-right:0;border-bottom:0;border-left:0"">
																<table class=""image_block block-1"" width=""100%"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace:0;mso-table-rspace:0"">
																	<tbody>
																		<tr>
																			<td class=""pad"" style=""width:100%;padding-right:0;padding-left:0"">
																				<div class=""alignment"" align=""center"" style=""line-height:10px"">
																					<a target=""_blank"" style=""outline:none"" tabindex=""-1"">
																						<img src=""https://doc-s.site/img/logo/docasLargeTransparency.png"" style=""display:block;height:auto;border:0;width:210px;max-width:100%"" width=""210"" alt=""Doc's logo"" title=""Doc's logo"">
																						</a>
																					</div>
																				</td>
																			</tr>
																		</tbody>
																	</table>
																</td>
															</tr>
														</tbody>
													</table>
												</td>
											</tr>
										</tbody>
									</table>
									<table class=""row row-2"" align=""center"" width=""100%"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace:0;mso-table-rspace:0"">
										<tbody>
											<tr>
												<td>
													<table class=""row-content stack"" align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace:0;mso-table-rspace:0;background-color:#efefef;color:#000;background-image:url(https://doc-s.site/img/pattern.png);background-repeat:repeat;background-size:auto;width:700px"" width=""700"">
														<tbody>
															<tr>
																<td class=""column column-1"" width=""100%"" style=""mso-table-lspace:0;mso-table-rspace:0;font-weight:400;text-align:left;padding-bottom:40px;padding-left:40px;padding-right:40px;padding-top:40px;vertical-align:top;border-top:0;border-right:0;border-bottom:0;border-left:0"">
																	<table class=""text_block block-1"" width=""100%"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace:0;mso-table-rspace:0;word-break:break-word"">
																		<tbody>
																			<tr>
																				<td class=""pad"" style=""padding-bottom:10px;padding-left:20px;padding-right:20px;padding-top:10px"">
																					<div style=""font-family:Tahoma,Verdana,sans-serif"">
																						<div class="""" style=""font-size:12px;font-family:Tahoma,Verdana,Segoe,sans-serif;mso-line-height-alt:14.399999999999999px;color:#555;line-height:1.2"">
																							<p style=""margin:0;font-size:14px;text-align:center;mso-line-height-alt:16.8px"">
																								<strong>
																									<span style=""font-size:24px;"">Cher(e) étudiant(e),</span>
																								</strong>
																							</p>
																						</div>
																					</div>
																				</td>
																			</tr>
																		</tbody>
																	</table>
																	<table class=""text_block block-2"" width=""100%"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace:0;mso-table-rspace:0;word-break:break-word"">
																		<tbody>
																			<tr>
																				<td class=""pad"" style=""padding-bottom:10px;padding-left:30px;padding-right:30px;padding-top:10px"">
																					<div style=""font-family:Verdana,sans-serif"">
																						<div class="""" style=""font-size:12px;font-family:Verdana,Geneva,sans-serif;mso-line-height-alt:18px;color:#555;line-height:1.5"">
																							<p style=""margin:0;font-size:14px;text-align:center;mso-line-height-alt:21px"">Merci de vous être inscrit(e) sur Doc's, le site dédié au partage de ressources pédagogiques pour les étudiants.</p>
																							<p style=""margin:0;font-size:14px;text-align:center;mso-line-height-alt:18px"">&nbsp;</p>
																							<p style=""margin:0;font-size:14px;text-align:center;mso-line-height-alt:21px"">Avant de pouvoir profiter pleinement de nos services, nous devons vérifier votre adresse e-mail.</p>
																						</div>
																					</div>
																				</td>
																			</tr>
																		</tbody>
																	</table>
																</td>
															</tr>
														</tbody>
													</table>
												</td>
											</tr>
										</tbody>
									</table>
									<table class=""row row-3"" align=""center"" width=""100%"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace:0;mso-table-rspace:0"">
										<tbody>
											<tr>
												<td>
													<table class=""row-content stack"" align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace:0;mso-table-rspace:0;background-color:#efefef;color:#000;width:700px"" width=""700"">
														<tbody>
															<tr>
																<td class=""column column-1"" width=""100%"" style=""mso-table-lspace:0;mso-table-rspace:0;font-weight:400;text-align:left;padding-bottom:10px;padding-left:10px;padding-right:10px;padding-top:10px;vertical-align:top;border-top:0;border-right:0;border-bottom:0;border-left:0"">
																	<table class=""text_block block-1"" width=""100%"" border=""0"" cellpadding=""10"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace:0;mso-table-rspace:0;word-break:break-word"">
																		<tbody>
																			<tr>
																				<td class=""pad"">
																					<div style=""font-family:Tahoma,Verdana,sans-serif"">
																						<div class="""" style=""font-size:14px;font-family:Tahoma,Verdana,Segoe,sans-serif;mso-line-height-alt:16.8px;color:#555;line-height:1.2"">
																							<p style=""margin:0;font-size:14px;text-align:center;mso-line-height-alt:16.8px"">
																								<span style=""font-size:20px;"">VOICI VOTRE CODE DE VERIFICATION UNIQUE</span>
																							</p>
																						</div>
																					</div>
																				</td>
																			</tr>
																		</tbody>
																	</table>
																	<table class=""text_block block-2"" width=""100%"" border=""0"" cellpadding=""10"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace:0;mso-table-rspace:0;word-break:break-word"">
																		<tbody>
																			<tr>
																				<td class=""pad"">
																					<div style=""font-family:Verdana,sans-serif"">
																						<div class="""" style=""font-size:14px;font-family:Verdana,Geneva,sans-serif;mso-line-height-alt:16.8px;color:#555;line-height:1.2"">
																							<p style=""margin:0;font-size:14px;text-align:center;mso-line-height-alt:16.8px"">
																								<span style=""font-size:20px;"">" +
																								code
																								+ @"</span>
																							</p>
																						</div>
																					</div>
																				</td>
																			</tr>
																		</tbody>
																	</table>
																</td>
															</tr>
														</tbody>
													</table>
												</td>
											</tr>
										</tbody>
									</table>
								</td>
							</tr>
";
			return layout(html);
		}
	}
}
